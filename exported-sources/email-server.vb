Window1.PushButton1.Action:
Sub Action()
  if anexo.visible then
    sendjpgmail(para.text,assunto.text,corpo.text,fd)
  else
    sendmail(para.text,assunto.text,corpo.text)
  end if
End Sub

Window1.BevelButton1.Action:
Sub Action()
  fd=getopenFolderItem("allfiles")
  anexo.caption="Anexo: "+fd.name
  anexo.visible=true
End Sub

EmailNotification.sendjpgmail:
Sub sendjpgmail(mailto as string, subject as string, message as string, jpg as folderItem)
  getconf
  smtp(ubound(smtp))=new smtpsocket
  smtp(ubound(smtp)).address=server
  smtp(ubound(smtp)).port=25
  smtp(ubound(smtp)).mailto=mailto
  smtp(ubound(smtp)).subject=subject
  smtp(ubound(smtp)).message=message
  smtp(ubound(smtp)).smtpuser=user
  smtp(ubound(smtp)).smtppasswd=passwd
  smtp(ubound(smtp)).mailfrom=email
  smtp(ubound(smtp)).attach=true
  smtp(ubound(smtp)).index=ubound(smtp)
  smtp(ubound(smtp)).picture=jpg
  smtp(ubound(smtp)).connect
  redim smtp(ubound(smtp)+1)
End Sub

EmailNotification.getconf:
Sub getconf()
  dim db as database
  dim cur as databasecursor
  db=openopenBaseDatabase("10.0.0.10","Octopus","admin","","REALbasic",true)
  if db<>nil then
    cur=new databasecursor
    cur=db.sQLSelect("select ServidorSmtp,UsuarioSmtp,SenhaSmtp,EmailDoSistema from parametros")
    server=cur.field("servidorsmtp").getString
    user=cur.field("usuariosmtp").getString
    passwd=cur.field("senhasmtp").getString
    email=cur.field("emaildosistema").getString
  end if
End Sub

EmailNotification.sendmail:
Sub sendmail(mailto as string, subject as string, message as string)
  getconf
  smtp(ubound(smtp))=new smtpsocket
  smtp(ubound(smtp)).address=server
  smtp(ubound(smtp)).port=25
  smtp(ubound(smtp)).mailto=mailto
  smtp(ubound(smtp)).subject=subject
  smtp(ubound(smtp)).message=message
  smtp(ubound(smtp)).smtpuser=user
  smtp(ubound(smtp)).smtppasswd=passwd
  smtp(ubound(smtp)).mailfrom=email
  smtp(ubound(smtp)).attach=false
  smtp(ubound(smtp)).index=ubound(smtp)
  smtp(ubound(smtp)).connect
  redim smtp(ubound(smtp)+1)
End Sub

smtpsocket.SendComplete:
Sub SendComplete(userAborted as Boolean)
  dim buffer as string
  
  if nextstep="sendfile" then
    if bina.eof then
      bina.close
      me.write chr(13)+chr(10)
      me.write chr(13)+chr(10)
      me.write "--Apple-Mail-1--826747196"+chr(13)+chr(10)
      me.write chr(13)+chr(10)
      me.write chr(13)+chr(10)
      me.write chr(13)+chr(10)
      me.write chr(13)+chr(10)+"."+chr(13)+chr(10)
      me.write "QUIT"+chr(13)+chr(10)
      me.close
      smtp.remove index
    else
      buffer=bina.Read(45)
      buffer=encodeBase64(buffer)
      me.write buffer+chr(13)+chr(10)
    end if
  end if
End Sub

smtpsocket.DataAvailable:
Sub DataAvailable()
  dim cod,crlf,data,lixo,buffer as string
  dim i as integer
  
  crlf=chr(13)+chr(10)
  
  data=me.readall
  
  data=replaceall(data,chr(13),"")
  
  for i=1 to countfields(data,chr(10))
    lixo=nthField(data,chr(10),i)
    cod=left(lixo,3)
    
    select case cod
    case "220"
      me.write "HELO <" + me.localaddress + ">" + crlf
      nextstep="auth"
      
      
    case "250" 
      if nextstep="auth" then
        me.write "AUTH LOGIN"+crlf
        nextstep="user"
      elseif NextStep="RCPT" then
        if countFields(mailto,",")=1 then
          me.write "RCPT TO: <"+mailto+">"+crlf
          NextStep="Data"
        else
          me.write "RCPT TO: <"+nthField(mailto,",",j)+">"+crlf
          j=j+1
          if j=countFields(mailto,",")+1 then
            NextStep="Data"
          end if
        end if
      elseif nextstep="data" then
        me.write "DATA" + crlf
        NextStep="Body"
      end if
    case "334"
      if nextstep="user" then
        me.write encodeBase64(smtpuser)+crlf
        nextstep="passwd"
      elseif nextstep="passwd" then
        me.write encodeBase64(smtppasswd)+crlf
        NextStep="Mail"
      end if
    case "235"
      me.write "MAIL FROM: <"+mailfrom+">"+ crlf
      NextStep="RCPT"
    case "354"
      nextstep=""
      me.write "From: "+chr(34)+"Octopus Server"+chr(34)+" "+"<"+mailfrom+">"+ crlf
      me.write "To: "+mailto+crlf
      me.write "Subject: "+subject+crlf
      me.write "Mime-Version: 1.0"+crlf
      me.write "Content-Type: multipart/mixed;"+crlf
      me.write chr(9)+"boundary="+chr(34)+"Apple-Mail-1--826747196"+chr(34)+crlf
      me.write "X-Mailer: Octopus Mail Notification. ver 1.0"+crlf
      me.write crlf
      me.write "--Apple-Mail-1--826747196"+crlf
      me.write "Content-Transfer-Encoding: 7bit"+crlf
      me.write "Content-Type: text/plain;"+crlf
      me.write chr(9)+"charset=US-ASCII;"+crlf
      me.write chr(9)+"format=flowed"+crlf
      me.write crlf
      me.write message+crlf
      me.write crlf
      me.write crlf
      me.write crlf
      me.write crlf
      me.write "--Apple-Mail-1--826747196"+crlf
      if attach then
        me.write "Content-Type: image/jpeg;"+crlf
        me.write chr(9)+"name="+chr(34)+picture.name+chr(34)+crlf
        me.write "Content-Transfer-Encoding: base64"+crlf
        me.write "Content-Disposition: attachment;"+crlf
        me.write chr(9)+"filename="+chr(34)+picture.name+chr(34)+crlf
        me.write crlf
        
        bina=picture.OpenAsBinaryFile(false)
        if bina<>nil then
          buffer=bina.Read(45)
          buffer=encodeBase64(buffer)
          me.write buffer+chr(13)+chr(10)
        end if
        nextstep="sendfile"
      else
        me.write "."+crlf
        me.write "QUIT"+crlf
        me.close
        smtp.remove index
      end if
    end select
  next
End Sub

smtpsocket.Connected:
Sub Connected()
  nextstep="HELO"
  j=1
End Sub

