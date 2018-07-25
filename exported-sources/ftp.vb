CommandSocket.list:
Sub list()
  lsock=new listDataSocket
  lsock.address=host
  lsock.port=portnum
  lsock.connect
End Sub

CommandSocket.pasvportdiscovery:
Sub pasvportdiscovery(byref data as string)
  dim temp, temp1,temp2 as string
  dim i as integer
  data=replaceall(data,chr(10),"")
  data=replaceall(data,chr(13),"")
  i=countFields(data," ")
  temp=nthField(data, " ",i)
  i=countFields(temp,",")
  temp1=nthField(temp,",",i-1)
  temp2=nthField(temp,",",i)
  temp2=left(temp2,len(temp2)-1)
  i=((val(temp1))*256)+(val(temp2))
  portnum=i
End Sub

CommandSocket.startdatasocket:
Sub startdatasocket()
  datasock=new dataSocket
  datasock.address=host
  datasock.port=portnum
  datasock.connect
End Sub

CommandSocket.Error:
Sub Error()
  msgbox "Command Socket Error: "+ str(me.lastErrorCode)
  status="idle"
End Sub

CommandSocket.DataAvailable:
Sub DataAvailable()
  dim cod,data,lixo,temp as string
  dim i as integer
  data=me.readall
  data = replaceall(data, chr(13), "")
  
  for i=1 to countFields(data,chr(10))
    lixo=nthField(data,chr(10),i)
    cod = left(lixo, 4)
    select case cod
      
    case "220 " //--------------------------------------------------------------------------------------//
      
      me.write "USER "+user+chr(10)
      
    case "331 " //--------------------------------------------------------------------------------------//
      
      me.write "PASS "+passwd+chr(10)
      
    case "230 " //--------------------------------------------------------------------------------------//
      
      me.write "MKD "+directory+chr(10)
      
    case "257 " //--------------------------------------------------------------------------------------//
      
      me.write "CWD "+directory+chr(10)
      
    case "250 " //--------------------------------------------------------------------------------------//
      
      me.write "PASV"+chr(10)
      status="list"
      
    case "227 " //--------------------------------------------------------------------------------------//
      
      pasvPortDiscovery(lixo)
      if status="list" then
        list
      elseif status="send" then
        me.write "TYPE I"+chr(10)
        startdatasocket
      end if
      
    case "200 " //--------------------------------------------------------------------------------------//
      
      if append=false then
        me.write "STOR "+filename+chr(10)
      elseif append=true then
        me.write "APPE "+filename+chr(10)
      end if
      
    case "530 " //--------------------------------------------------------------------------------------//
      
      me.close
      ftp.connectbutton.caption="Conectar"
      msgbox "Nome de usu‡rio ou senha inv‡lidos."    // <-- CONVERTED
      
    case "550 " //--------------------------------------------------------------------------------------//
      
      me.write "CWD "+directory+chr(10)
      
    case "226 " //--------------------------------------------------------------------------------------//
      if stop<>true then
        if status="idle" then
          ftp.chasingArrows1.visible=false
          if send=true then
            checkQueue
          end if
        elseif status="list" then
          me.write "PASV"+chr(10)
        elseif status="send" then
          status="list"
          me.write "PASV"+chr(10)
        end if
        if sendaftercomp=true then
          settransfer
          sendaftercomp=false
        end if
      else
        if status="list" then
          me.write "PASV"+chr(10)
        end if
      end if
    case "221 " //--------------------------------------------------------------------------------------//
      
      ftp.connectbutton.caption="Conectar"
      me.close
      
    end select //--------------------------------------------------------------------------------------//
    
    
    if cod="226 " then
      ftp.staticText1.text="Pronto."
    end if
  next
  data=""
End Sub

CommandSocket.Connected:
Sub Connected()
  ftp.staticText1.text="Conectado"
End Sub

ListDataSocket.Listfiles:
Sub Listfiles()
  dim i as integer
  dim name, bytes as string
  dim lixo as string
  dim xx as integer
  
  ftp.listbox2.deleteAllRows
  lixo=nthField(data,chr(10),1)
  data=replaceall(data,lixo,"")
  data=replace(data,chr(10),"")
  data = replaceall(data, chr(13), "")
  ftp.listbox2.visible=false
  for i=0 to countfields(Data, chr(10))-2
    lixo=NthField(data,chr(10),i+1)
    lixo=replaceall(lixo,"     "," ")
    lixo=replaceall(lixo,"    "," ")
    lixo=replaceall(lixo,"   "," ")
    lixo=replaceall(lixo,"  "," ")
    bytes=NthField(lixo," ",5)
    bytes=trim(bytes)
    name=lixo
    lixo=""
    for xx=1 to 8 
      lixo=lixo+" "+NthField(name," ",xx)
    next
    lixo=trim(lixo)
    name=replaceall(name,lixo,"")
    name=trim(name)
    ftp.listbox2.addrow name
    ftp.listbox2.rowPicture(ftp.listbox2.lastIndex) = oct
    ftp.listbox2.cell(ftp.listbox2.lastIndex,1)=filesizestringlocal(bytes)
    ftp.listbox2.cell(ftp.listbox2.lastIndex,2)=bytes
    name=""
  next
  ftp.listbox2.visible=true
  me.close
End Sub

ListDataSocket.filesizestringlocal:
Function filesizestringlocal(size as string) As string
  dim i as integer
  dim b as double
  dim a as string
  
  i=val(size)
  
  if i>0 and i<1024 then
    a=format(i,"####\ \B\y\t\e\s")
    return a
  elseif i>=1024 and i<1048576 then
    b=i/1024
    a=format(b,"####.0#\ \K\B")
    return a
  elseif i>=1048576 and i<1073741824 then
    b=(i/1024)/1024
    a=format(b,"####.0#\ \M\B")
    return a
  elseif i>=1073741824 and i<1000000000000 then
    b=((i/1024)/1024)/1024
    a=format(b,"####.0#\ \G\B")
    return a
  end if
  
End Function

ListDataSocket.Error:
Sub Error()
  select case me.lastErrorCode
  case 102
  else
    msgBox "ListDataSocket Error: "+str(me.lastErrorCode)
  end select
End Sub

ListDataSocket.DataAvailable:
Sub DataAvailable()
  dim i as integer
  for i=0 to 100
    data = data+me.readall
  next
  ListFiles
End Sub

ListDataSocket.Connected:
Sub Connected()
  if comsock<>nil then
    if comsock.isConnected then
      comsock.write "LIST"+chr(10)
      comsock.status="idle"
    end if
  end if
  if csocket<>nil then
    if csocket.isConnected then
      csocket.write "LIST"+chr(10)
      csocket.status="idle"
    end if
  end if
End Sub

Module.filesizestring:
Function filesizestring(fd as folderItem) As string
  dim i as integer
  dim b as double
  dim a as string
  
  i=fd.length+fd.resourceForkLength
  
  if i>0 and i<1024 then
    a=format(i,"####\ \B\y\t\e\s")
    return a
  elseif i>=1024 and i<1048576 then
    b=i/1024
    a=format(b,"####.0#\ \K\B")
    return a
  elseif i>=1048576 and i<1073741824 then
    b=(i/1024)/1024
    a=format(b,"####.0#\ \M\B")
    return a
  elseif i>=1073741824 and i<1000000000000 then
    b=((i/1024)/1024)/1024
    a=format(b,"####.0#\ \G\B")
    return a
  end if
  
End Function

Module.addfiletodb:
Sub addfiletodb(fd as folderItem)
  dim cur as recordSet
  dim rec as databaserecord
  dim i as integer
  
  cur=ftpdb.sQLSelect("Select id from counter")
  i=cur.field("id").IntegerValue
  
  if i=0 then
    rec=new databaserecord
    rec.IntegerColumn("id")=1
    ftpdb.insertRecord("counter",rec)
    ftpdb.commit
  else
    cur.edit
    cur.field("id").IntegerValue=i+1
    cur.update
  end if
  ftpdb.commit
  cur.close
  rec=new databaserecord
  rec.column("name")=fd.displayName
  rec.column("size")=filesizestring(fd)
  rec.column("status")="N‹o Compactado"    // <-- CONVERTED
  rec.column("path")=fd.absolutePath
  rec.column("compressed")="no"
  rec.column("realsize")=format((fd.length+fd.resourceForkLength),"################")
  rec.column("id")=str(i)
  ftpdb.insertRecord("queue", rec)
  ftpdb.commit
  ftpdb.close
End Sub

Module.checkfilenamechar:
Function checkfilenamechar(fd as folderItem) As string
  dim i as integer
  dim a as string
  dim x as integer
  dim y as integer
  dim name,lixo as string
  lixo=fd.name
  
  name=""
  y=0
  i=len(lixo)
  for x=1 to i
    a=mid(lixo,x,1)
    y=asc(a)
    if (y>=48 and y<=57) or (y>=65 and y<=90) or (y>=97 and y<=122) or y=46 then
      name=name+a
    else
      name=name+"_"
    end if
  next
  name=lowercase(name)
  return name
End Function

Module.restartcomsocket:
Sub restartcomsocket()
  comsock=new commandSocket
  comsock.address=host
  comsock.port=21
  comsock.connect
End Sub

Module.comparefiles:
Function comparefiles() As boolean
  dim xx,yy as integer
  dim a,b as string
  
  if ftp.listbox2.listcount<>0 then
    a=ftp.listbox1.cell(0,0)
    for xx=0 to ftp.listbox2.listcount-1
      b=ftp.listbox2.cell(xx,0)
      yy=StrComp(a,b,0)
      if yy=0 then
        a=ftp.listbox1.cell(0,4)
        b=ftp.listbox2.cell(xx,2)
        yy=StrComp(a,b,0)
        if yy=0 then
          comsock.append=false
          return true
        else
          comsock.append=true
          remotefilesize=val(ftp.listbox2.cell(xx,2))
          return false
        end if
      else
      end if
    next
  end if
  return false
End Function

Module.settransfer:
Sub settransfer()
  dim fd as folderItem
  if comsock<>nil then
    if comsock.IsConnected=true then
      if ftp.listbox1.listcount<>0 then
        fd=GetFolderItem(ftp.listbox1.cell(0,6))
        filename=fd.name
        if comparefiles=false then
          If fd <> Nil Then
            comsock.send=true
            comsock.status="send"
            comsock.write "PASV"+chr(10)
            ftp.staticText1.text="Iniciando a Transferncia."    // <-- CONVERTED
          end if
        else
          terminate(fd)
          checkqueue
        end if
      end if
    else
      connecttoserver
    end if
  else
    connecttoserver
  end if
End Sub

Module.checkqueue:
Sub checkqueue()
  if ftp.listbox1.ListCount=0 then
    if comsock<>nil then
      if comsock.isConnected then
        comsock.send=false
        comsock.write "QUIT"+chr(10)
        ftp.chasingArrows1.visible=false
      end if
    end if
    datafinal
    writelog
  else
    ftp.chasingArrows1.visible=true
    settransfer
  end if
End Sub

Module.connecttoserver:
Sub connecttoserver()
  ftp.statictext1.text="Conectando ao servidor. Aguarde..."
  restartcomsocket
  ftp.connectbutton.caption="Desconectar"
  ftp.connectbutton.default=false
  ftp.chasingArrows1.visible=true
End Sub

Module.terminate:
Sub terminate(arq as folderItem)
  dim ext as string
  
  ext=right(arq.name,3)
  
  removefromDB(0)
  ftp.listbox1.removeRow 0
  ftp.progressBar1.value=0
  comsock.append=false
  if ext="oct" then
    arq.delete
  end if
End Sub

Module.theend:
Sub theend()
  dim cur as recordSet
  
  cur=ftpdb.sQLSelect("select status from job where os='"+os+"'")
  cur.edit
  cur.field("status").setString("ok")
  cur.update
  ftpdb.commit
  ftpdb.sQLExecute("delete from counter")
  ftpdb.commit
  ftpdb.sQLExecute("delete from Files")
  ftpdb.commit
  ftpdb.close
End Sub

Module.writelog:
Sub writelog()
  dim cur as recordSet
  dim contato,obs,email,extencao as string
  dim i as integer
  dim file as folderItem
  dim filestream as textoutputStream
  
  cur=ftpdb.sQLSelect("select * from job where os='"+os+"'")
  contato=cur.field("contato").getString
  obs=cur.field("obs").getString
  email=cur.field("email").getString
  cur.close
  ftpdb.close
  i=lognumber
  if (i>0 and i<=9) then
    extencao="00"+str(i)
  elseif i>=10 and i<=99 then
    extencao="0"+str(i)
  elseif i>=100 and i<=999 then
    extencao=str(i)
  end if
  
  
  file=getfolderItem(os+".log."+extencao)
  fileStream=file.CreateTextFile
  fileStream.WriteLine "01"+chr(9)+"NUMERO DA OS"+chr(9)+os
  fileStream.WriteLine "02"+chr(9)+"CONTATO"+chr(9)+contato
  fileStream.WriteLine "03"+chr(9)+"EMAIL"+chr(9)+email
  fileStream.WriteLine "04"+chr(9)+"OBS"+chr(9)+obs
  fileStream.WriteLine "05"+chr(9)+"DIR"+chr(9)+directory
  logfile=file
  i=0
  cur=ftpdb.sQLSelect("select name from Files")
  while not cur.eof
    fileStream.WriteLine "06"+chr(9)+"FILENAME"+chr(9)+cur.field("name").getString
    i=i+1
    cur.movenext
  wend
  
  cur.close
  ftpdb.close
  fileStream.WriteLine "07"+chr(9)+"ARQ.#"+chr(9)+str(i)
  fileStream.Close
  sendlog
End Sub

Module.sendlog:
Sub sendlog()
  logcomsock=new logcommandSocket
  logcomsock.address=host
  logcomsock.port=21
  logcomsock.connect
End Sub

Module.addcompressedfiletodb:
Sub addcompressedfiletodb(fd as folderItem)
  dim cur as recordSet
  dim rec as databaserecord
  dim i as integer
  
  cur=ftpdb.sQLSelect("Select id from counter")
  i=cur.field("id").IntegerValue
  
  if i=0 then
    rec=new databaserecord
    rec.IntegerColumn("id")=1
    ftpdb.insertRecord("counter",rec)
    ftpdb.commit
  else
    cur.edit
    cur.field("id").IntegerValue=i+1
    cur.update
  end if
  ftpdb.commit
  cur.close
  rec=new databaserecord
  rec.column("name")=fd.displayName
  rec.column("size")=filesizestring(fd)
  rec.column("status")="Compactado"
  rec.column("path")=fd.absolutePath
  rec.column("sent")="false"
  rec.column("compressed")="yes"
  rec.column("realsize")=format((fd.length+fd.resourceForkLength),"################")
  rec.column("id")=str(i)
  ftpdb.insertRecord("queue", rec)
  ftpdb.commit
  ftpdb.close
End Sub

Module.changedbfileinfo:
Sub changedbfileinfo(f1 as folderItem, f2 as folderItem, id as string)
  dim cur as recordSet
  
  cur=ftpdb.SQLSelect("select * from queue where id='"+id+"'")
  cur.edit
  cur.field("name").setString(f2.Name)
  cur.field("path").setString(f2.absolutePath)
  cur.field("size").setString(filesizestring(f2))
  cur.field("status").setString("Compactado")
  cur.field("compressed").setString("yes")
  cur.field("realsize").setString(format((f2.length+f2.resourceForkLength),"################"))
  cur.update
  ftpdb.commit
  cur.close
  ftpdb.close
End Sub

Module.refreshqueue:
Sub refreshqueue()
  dim cur as recordSet
  dim i as Integer
  dim v as String
  ftp.listbox1.deleteAllRows
  cur=ftpdb.SQLSelect("Select * from queue")
  While not cur.eof
    ftp.listbox1.addrow cur.Field("name").getstring
    i=ftp.listbox1.lastIndex
    ftp.listbox1.cell(i,1)=cur.Field("size").getstring
    ftp.listbox1.cell(i,2)=cur.Field("status").getstring
    ftp.listbox1.cell(i,6)=cur.Field("path").getstring
    ftp.listbox1.cell(i,3)=cur.Field("compressed").getstring
    ftp.listbox1.cell(i,4)=cur.field("realsize").getString
    ftp.listbox1.cell(i,5)=cur.field("id").getString
    cur.MoveNext
  wend
  cur.Close
  ftpdb.close
End Sub

Module.RemovefromDB:
Sub RemovefromDB(i as integer)
  dim cur as recordSet
  cur=ftpdb.sQLSelect("select * from queue where id='"+ftp.listbox1.cell(i,5)+"'")
  cur.deleteRecord
  ftpdb.commit
  cur.close
  ftpdb.close
End Sub

Module.datafinal:
Sub datafinal()
  dim recset as recordSet
  dim now as date
  now=new date
  recset=ftpdb.sQLSelect("select termino from job where os="+chr(34)+os+chr(34))
  recset.edit
  recset.field("termino").setString(now.LongDate+chr(9)+now.longtime)
  recset.Update
  recset.Close
End Sub

DataSocket.addfiletolog:
Sub addfiletolog()
  dim rec as databaserecord
  rec=new databaserecord
  rec.column("name")=f.name
  ftpdb.insertRecord("Files",rec)
  ftpdb.commit
  ftpdb.close
End Sub

DataSocket.SendComplete:
Sub SendComplete(userAborted as Boolean)
  dim buffer as string
  if stop then 
    if comsock<>nil then
      if comsock.isConnected then
        comsock.status="list"
      end if
    end if
    me.close
  else
    if (binstream.eof) then
      addfiletolog
      ftp.stopbutton.enabled=false
      ftp.timer1.mode=0
      ftp.txtvelocidade.visible=false
      ftp.txtprogresso.visible=false
      binstream.close
      terminate(f)
      me.close
    else
      buffer = binstream.read(speed)
      me.write(buffer)
      FileSent = FileSent + len(buffer)
      if ftp.txtprogresso.visible=false then
        ftp.txtprogresso.visible=true
      end if
      if comsock.append=true then
        remotefilesize=remotefilesize+len(buffer)
        ftp.progressBar1.value = (remotefilesize*100)/FileSize
        ftp.txtprogresso.text="Progresso:      "+format(((remotefilesize*100)/datasock.Filesize),"##.0\ \%")
      else
        ftp.progressBar1.value = (FileSent*100)/FileSize
        ftp.txtprogresso.text="Progresso:      "+format(((datasock.Filesent*100)/datasock.Filesize),"##.0\ \%")
      end if
    end if
  end if
End Sub

DataSocket.Error:
Sub Error()
  msgBox "Data Socket Error: "+str(me.lastErrorCode)
End Sub

DataSocket.Connected:
Sub Connected()
  dim buffer as string
  ftp.staticText1.text="Enviando..."
  ftp.stopbutton.enabled=true
  
  f=getfolderItem(ftp.listbox1.cell(0,6))
  clock=new date
  if (f<>nil) then
    binstream = f.openasBinaryFile(false)
    if (binstream <> nil) then
      FileSize = f.length
      if comsock.append=true then
        binstream.Position=remotefilesize
      end if
      buffer = binstream.read(speed)
      me.write(buffer)
      FileSent = len(buffer)
      bps = filesent
      if comsock.append=true then
        ftp.progressBar1.value=(remotefilesize*100)/FileSize
      else
        ftp.progressBar1.Value = (FileSent*100)/FileSize
      end if
      ftp.timer1.mode=2
    else
      terminate(f)
      me.close
    end if
  end if
End Sub

LogCommandSocket.PasvPortDiscovery:
Sub PasvPortDiscovery(byref data as string)
  dim temp, temp1,temp2 as string
  dim i as integer
  data=replaceall(data,chr(10),"")
  data=replaceall(data,chr(13),"")
  i=countFields(data," ")
  temp=nthField(data, " ",i)
  i=countFields(temp,",")
  temp1=nthField(temp,",",i-1)
  temp2=nthField(temp,",",i)
  temp2=left(temp2,len(temp2)-1)
  i=((val(temp1))*256)+(val(temp2))
  portnum=i
End Sub

LogCommandSocket.startlogdatasocket:
Sub startlogdatasocket()
  logdatasock=new logdataSocket
  logdatasock.address=host
  logdatasock.port=portnum
  logdatasock.connect
End Sub

LogCommandSocket.Connected:
Sub Connected()
  ftp.connectbutton.enabled=false
  ftp.staticText1.text="Enviando Log..."
End Sub

LogCommandSocket.Error:
Sub Error()
  
  msgbox "Command Socket Error: "+ str(me.lastErrorCode)
  
End Sub

LogCommandSocket.DataAvailable:
Sub DataAvailable()
  dim cod,data,lixo,temp as string
  dim i as integer
  data=me.readall
  data = replaceall(data, chr(13), "")
  
  for i=1 to countFields(data,chr(10))
    lixo=nthField(data,chr(10),i)
    cod = left(lixo, 4)
    select case cod
      
    case "220 " //--------------------------------------------------------------------------------------//
      
      me.write "USER "+user+chr(10)
      
    case "331 " //--------------------------------------------------------------------------------------//
      
      me.write "PASS "+passwd+chr(10)
      
    case "230 " //--------------------------------------------------------------------------------------//
      
      me.write "PASV"+chr(10)
      
    case "227 " //--------------------------------------------------------------------------------------//
      
      pasvPortDiscovery(lixo)
      me.write "TYPE A"+chr(10)
      
    case "200 " //--------------------------------------------------------------------------------------//
      
      startlogdatasocket
      me.write "STOR "+logfile.name+chr(10)
      
    case "226 " //--------------------------------------------------------------------------------------//
      
      me.close
      logwindow.show
      ftp.close
    end select
  next
End Sub

LogDataSocket.Error:
Sub Error()
  msgBox "Data Socket Error: "+str(me.lastErrorCode)
End Sub

LogDataSocket.SendComplete:
Sub SendComplete(userAborted as Boolean)
  dim buffer as string
  
  if (binstream.eof) then
    binstream.close
    logfile.delete
    me.close
  else
    buffer = binstream.read(speed)
    me.write(buffer)
  end if
End Sub

LogDataSocket.Connected:
Sub Connected()
  dim buffer as string
  
  if (logfile<>nil) then
    binstream = logfile.openasBinaryFile(false)
    if (binstream <> nil) then
      buffer = binstream.read(speed)
      me.write(buffer)
    else
      me.close
    end if
  end if
End Sub

Restorquit.PushButton1.Action:
Sub Action()
  dim i as integer
  i=rnd*1000000
  directory=str(i)
  self.close
  ftp.close
  info.show
End Sub

Restorquit.PushButton2.Action:
Sub Action()
  quit
End Sub

Restorquit.Canvas1.Paint:
Sub Paint(g As Graphics)
  g.drawcautionIcon 0,0
End Sub

Info.checkosexistence:
Function checkosexistence() As Boolean
  dim cur as recordSet
  
  cur=ftpdb.sQLSelect("select os from job")
  while not cur.eof
    if cur.field("os").getString=popupMenu1.rowTag(popupMenu1.listindex) then
      return true
    end if
    cur.movenext
  wend
  return false
  cur.close
  ftpdb.close
End Function

Info.plusone:
Sub plusone()
  dim cur as recordSet
  dim i as integer
  
  cur=ftpdb.sQLSelect("select number from job where os='"+popupMenu1.rowTag(popupMenu1.listindex)+"'")
  i=cur.field("number").integerValue
  i=i+1
  cur.edit
  cur.field("number").integerValue=i
  lognumber=i
  cur.Update
  cur.close
  ftpdb.close
End Sub

Info.updatejobinfo:
Sub updatejobinfo()
  dim now as date
  dim cur as recordSet
  
  cur=ftpdb.sQLSelect("Select * from job where os='"+os+"'")
  cur.edit
  cur.field("obs").setString(cobs.text)
  cur.field("directory").setString(directory)
  cur.field("contato").setString(ccontato.text)
  cur.field("email").setString(cemail.text)
  cur.field("status").setString("new")
  now=new date
  cur.field("inicio").setstring(now.LongDate+chr(9)+now.longtime)
  cur.field("termino").setString("")
  cur.update
  ftpdb.commit
  cur.close
  ftpdb.close
End Sub

Info.newjob:
Sub newjob()
  dim rec as databaserecord
  dim now as date
  rec=new databaserecord
  rec.Column("os")=popupMenu1.rowTag(popupMenu1.listindex)
  rec.column("contato")=ccontato.text
  rec.column("obs")=cobs.text
  rec.column("email")=cemail.text
  rec.column("status")="new"
  rec.column("directory")=directory
  rec.integerColumn("number")=1
  now=new date
  rec.Column("inicio")=now.LongDate+chr(9)+now.longtime
  lognumber=1
  ftpdb.insertRecord("job",rec)
  ftpdb.commit
  ftpdb.close
End Sub

Info.Ccontato.Open:
Sub Open()
  me.text=nomeCompleto
End Sub

Info.PushOk.Action:
Sub Action()
  if popupMenu1.rowTag(popupMenu1.listindex)="" or ccontato.text="" or cemail.text="" then
    msgBox "N¼ da O.S., Contato e Email s‹o campos obrigat—rios."    // <-- CONVERTED
  else
    if checkosexistence=true then
      os=popupMenu1.rowTag(popupMenu1.listindex)
      plusone
      updatejobinfo
    else
      newjob
      os=popupMenu1.rowTag(popupMenu1.listindex)
    end if
    self.close
    ftp.show
  end if
End Sub

Info.PushCancel.Action:
Sub Action()
  'cos.text=""
  'ccontato.text=""
  'cemail.text=""
  'cobs.text=""
  'cos.setfocus
  quit
End Sub

Info.cemail.Open:
Sub Open()
  me.text=email
End Sub

Info.cemail.KeyDown:
Function KeyDown(Key As String) As Boolean
  if key=" " then
    //me.text=trim(me.text)
    //me.text=replaceall(me.text," ","")
    return true
  end if
End Function

Info.PopupMenu1.Open:
Sub Open()
  dim r as recordSet
  
  r=octopus.sqLSelect("Select Codigo, Descricao from trabalhos where DescricaoStatus='ATIVO' and CodCliente='"+codigodaempresa+"'")
  if r<>nil then
    while not r.eof
      me.addrow r.field("Codigo").getString+"  -  "+r.field("Descricao").getString
      me.rowTag(me.listCount-1)=r.field("Codigo").getString
      r.movenext
    wend
    r.close
  end if
  me.listindex=0
End Sub

Info.PopupMenu1.Change:
Sub Change()
  os=me.rowTag(me.listindex)
End Sub

WinCon.putconsetupindatabase:
Sub putconsetupindatabase()
  dim rec as databaserecord
  rec=new databaserecord
  rec.Column("host")=chost.text
  rec.column("user")=cuser.text
  rec.column("passwd")=cpasswd.text
  rec.integerColumn("speed")=speed
  ftpdb.insertRecord("consetup",rec)
  ftpdb.commit
  ftpdb.close
End Sub

WinCon.updateconsetupindatabase:
Sub updateconsetupindatabase()
  dim cur as recordSet
  cur=ftpdb.sQLSelect("select * from consetup")
  cur.edit
  cur.field("host").setString(chost.text)
  cur.field("user").setString(cuser.text)
  cur.field("passwd").setString(cpasswd.text)
  cur.field("speed").integerValue=speed
  cur.update
  ftpdb.commit
  cur.close
  ftpdb.close
End Sub

WinCon.Open:
Sub Open()
  select case speed
  case 7168
    c56kbps.value=true
  case 16384
    c128kbps.value=true
  case 32768
    c256kbps.value=true
  case 65536
    c512kbps.value=true
  case 98304
    c768kbps.value=true
  case 131072
    c1mbps.value=true
  end select
  
  chost.text=host
  cuser.text=user
  cpasswd.text=passwd
End Sub

WinCon.PushOk.Action:
Sub Action()
  if host="" then
    if chost.text<>"" and cuser.text<>"" and cpasswd.text<>"" then
      putconsetupindatabase
      host=chost.text
      user=cuser.text
      passwd=cpasswd.text
      self.close
    else
      msgBox "Os campos devem ser preenchidos."
    end if
  else
    updateconsetupindatabase
    host=chost.text
    user=cuser.text
    passwd=cpasswd.text
    self.close
  end if
End Sub

WinCon.PushCancel.Action:
Sub Action()
  self.close
End Sub

WinCon.PushButton1.Action:
Sub Action()
  ftpdb.sQLExecute("delete from queue")
  ftpdb.sQLExecute("delete from consetup")
  ftpdb.sQLExecute("delete from counter")
  ftpdb.sQLExecute("delete from Files")
  ftpdb.sQLExecute("delete from job")
  ftpdb.commit
  refreshqueue
End Sub

WinCon.c56kbps.Action:
Sub Action()
  if me.value=true then
    speed=7168
  end if
End Sub

WinCon.c128kbps.Action:
Sub Action()
  if me.value=true then
    speed=16384
  end if
End Sub

WinCon.c256kbps.Action:
Sub Action()
  if me.value=true then
    speed=32768
  end if
End Sub

WinCon.c512kbps.Action:
Sub Action()
  if me.value=true then
    speed=65536
  end if
End Sub

WinCon.c768kbps.Action:
Sub Action()
  if me.value=true then
    speed=98304
  end if
End Sub

WinCon.c1mbps.Action:
Sub Action()
  if me.value=true then
    speed=131072
  end if
End Sub

OCTcompress.octopuscompress:
Sub octopuscompress(original as folderItem, id as string)
  dim readfromfile,writetofile as binaryStream
  dim compressed as folderItem
  #if targetcarbon then
    dim readresource as resstreamMBS
  #endif
  dim parametros,data as string
  dim filetotal,position,controle as double
  dim i as integer
  
  if stop=false then
    if original<>nil then
      if original.mactype<>"OCT!" and original.maccreator<>"OCTO" then
        #if targetcarbon then
          filetotal=original.length+original.resourceForkLength
        #else
          filetotal=original.length
        #endif
        
        readfromfile=original.openasbinaryFile(false)
        
        #if targetcarbon then
          readresource=original.openasResStream(false)
        #endif
        
        compressed=original.TemporaryFolder.child(checkfilenamechar(original)+".oct")
        if compressed.exists then
          compressed.delete
        end if
        
        writetofile=compressed.createbinaryFile("Octopus Compress")
        
        #if targetcarbon then
          parametros="<header-:-type=file"+chr(9)+"name="+original.name+chr(9)+"path="+original.name+chr(9)+"maccreator="+original.maccreator+chr(9)+"mactype="+original.mactype+chr(9)+"flags="+format(original.getFileFlags,"00000000")+chr(9)+"os=mac>"
        #endif
        
        #if targetwin32 then
          parametros="<header-:-type=file"+chr(9)+"name="+original.name+chr(9)+"path="+original.name+chr(9)+"os=pc>"
        #endif
        
        
        i=lenb(parametros)
        writetofile.write format(i,"0000")
        writetofile.write parametros
        
        #if targetcarbon then
          data=readresource.read(readresource.Length)
          data=compress(data,9)
          writetofile.write format(lenb(data),"00000000")
          writetofile.write data
          position=readresource.length
          readresource.close
        #endif
        
        wincomp.progressBar2.value=position*100/filetotal
        
        while not readfromfile.eof
          data=compress(readfromfile.read(1048576),9)
          controle=lenB(data)
          writetofile.write format(controle,"00000000")
          writetofile.write data
          position=position+1048576
          wincomp.progressBar2.value=position*100/filetotal
          if stop=true then
            return
          end if
        wend
        writetofile.write "*=NEXT=*"
        readfromfile.close
        writetofile.close
        compressed.mactype="OCT!"
        compressed.maccreator="OCTO"
        if stop=false then
          changedbfileinfo(original,compressed,id)
        end if
      end if
    end if
  end if
End Sub

OCTcompress.mapsubfolders:
Sub mapsubfolders(fd as folderItem)
  dim i as integer
  dim lixo as folderItem
  for i=1 to fd.count
    lixo=fd.trueitem(i)
    if lixo.isreadable then
      if lixo.visible then
        if lixo.directory then
          folders(ubound(folders))=folders(ubound(folders))+lixo.absolutePath+chr(10)
          mapsubfolders(lixo)
        end if
      end if
    end if
  next
End Sub

OCTcompress.termina:
Sub termina()
  dim i as integer
  
  ftp.chasingArrows1.visible=false
  if stop=false then
    for i=0 to ftp.listbox1.listcount-1
      if ftp.listbox1.cell(i,3)<>"no" then
        yes=true
      end if
    next
    if yes=true then
      if ftp.checkbox1.value=true then
        if ftp.connectbutton.caption="Conectar" then
          sendaftercomp=true
          ftp.timer2.mode=1
        else
          sendaftercomp=true
          settransfer
        end if
      end if
    end if
  end if
  wincomp.timer1.mode=1
End Sub

OCTcompress.octopusfoldercompress:
Sub octopusfoldercompress()
  dim i,j as integer
  dim path as string
  
  for j=0 to ubound(folders)
    root=nthField(folders(j),chr(10),1)
    criaarq(root)
    for i=1 to countfields(folders(j),chr(10))-1
      path=nthField(folders(j),chr(10),i)
      octopusfolderfilescompress(path)
    next
    writetofolder.close
    changefolderdbinfo(nthField(folders(j),chr(10),1))
  next
End Sub

OCTcompress.criaarq:
Sub criaarq(path as string)
  compressedfolder=getfolderItem(path)
  compressedfolder=compressedfolder.temporaryFolder.child(checkfilenamechar(compressedfolder)+".oct")
  writetofolder=compressedfolder.createbinaryFile("Octopus Compress")
End Sub

OCTcompress.octopusfolderfilescompress:
Sub octopusfolderfilescompress(path as string)
  dim readfromfile as binaryStream
  dim fd,original as folderItem
  #if targetcarbon then
    dim readresource as resstreamMBS
  #endif
  dim parametros,data as string
  dim controle as double
  dim i,j as integer
  
  fd=getfolderItem(path)
  data="<header-:-type=folder"+chr(9)+"name="+fd.name+chr(9)+"path="+replace(fd.absolutePath,root,"")+chr(9)+"qtde="+str(count)+">"
  i=lenb(data)
  writetofolder.write format(i,"0000")
  writetofolder.write data
  
  if stop=false then
    for j=1 to fd.count
      original=fd.trueitem(j)
      if not original.directory then
        if original<>nil then
          if original.mactype<>"OCT!" and original.maccreator<>"OCTO" then
            if left(original.name,1)<>"." then
              readfromfile=original.openasbinaryFile(false)
              
              #if targetcarbon then
                readresource=original.openasResStream(false)
              #endif
              
              #if targetcarbon then
                parametros="<header-:-type=file"+chr(9)+"name="+original.name+chr(9)+"path="+replace(original.absolutePath,root,"")+chr(9)+"maccreator="+original.maccreator+chr(9)+"mactype="+original.mactype+chr(9)+"flags="+format(original.getFileFlags,"00000000")+chr(9)+"os=mac>"
              #endif
              
              #if targetwin32 then
                parametros="<header-:-type=file"+chr(9)+"name="+original.name+chr(9)+"path="+replace(original.absolutePath,root,"")+chr(9)+"os=pc>"
              #endif
              
              i=lenb(parametros)
              writetofolder.write format(i,"0000")
              writetofolder.write parametros
              
              #if targetcarbon then
                if readresource<>nil then
                  data=readresource.read(readresource.Length)
                  data=compress(data,9)
                  writetofolder.write format(lenb(data),"00000000")
                  writetofolder.write data
                  readresource.close
                else
                  writetofolder.write format(0,"00000000")
                end if
              #endif
              
              
              while not readfromfile.eof
                data=compress(readfromfile.read(1048576),9)
                controle=lenB(data)
                writetofolder.write format(controle,"00000000")
                writetofolder.write data                  
                if stop=true then
                  return
                end if
              wend
              writetofolder.write "*=NEXT=*"
              done=done+1
              readfromfile.close
              wincomp.progressBar2.value=done*100/count
            end if
          end if
        end if
      end if
    next
  end if
End Sub

OCTcompress.changefolderdbinfo:
Sub changefolderdbinfo(path as string)
  dim i as integer
  dim fd as folderItem
  
  for i=0 to ftp.listbox1.listcount-1
    if stop=false then
      if strcomp(path,ftp.listbox1.cell(i,6),0)=0 then
        fd=getfolderItem(ftp.listbox1.cell(i,6))
        changedbfileinfo(fd,compressedfolder,ftp.listbox1.cell(i,5))
        refreshqueue
      end if
    end if
  next
End Sub

OCTcompress.countfiles:
Sub countfiles(path as string)
  dim pasta,fd as folderItem
  dim i as integer
  
  pasta=getFolderItem(path)
  for i=1 to pasta.count
    fd=pasta.trueItem(i)
    if fd.directory then
      countfiles(fd.absolutePath)
    else
      if left(fd.name,1)<>"." then
        count=count+1
      end if
    end if
  next
End Sub

OCTcompress.Run:
Sub Run()
  dim original as folderItem
  dim i,j as integer
  dim id as string
  
  ftp.chasingArrows1.visible=true
  
  for i=0 to ftp.listbox1.ListCount-1
    original=getFolderItem(ftp.listbox1.cell(i,6))
    countfiles(original.absolutePath)
    if original.directory then
      folderexistence=true
      folders(ubound(folders))=original.absolutePath+chr(10)
      mapsubfolders(original)
      redim folders(ubound(folders)+1)
    end if
  next
  
  for i=0 to ftp.listbox1.ListCount-1
    if stop=false then
      if ftp.listbox1.cell(i,3)="no" then
        original=getFolderItem(ftp.listbox1.cell(i,6))
        id=ftp.listbox1.cell(i,5)
        wincomp.staticText2.text=original.name
        if not original.directory then
          octopuscompress(original,id)
          //i=-1
          j=j+1
          wincomp.progressBar1.value=j*100/ftp.listbox1.ListCount
          ftp.listbox1.cell(i,3)="yes"
          ftp.listbox1.cell(i,2)="Compactado"
        end if
      end if
    else
      i=ftp.listbox1.ListCount-1
    end if
  next
  refreshqueue
  if folderexistence then
    octopusfoldercompress
  end if
  
  termina
End Sub

wincomp.Open:
Sub Open()
  chasingArrows1.enabled=true
  chasingArrows1.visible=true
End Sub

wincomp.PushButton1.Action:
Sub Action()
  stop=true
  chasingArrows1.visible=false
  sendaftercomp=false
End Sub

wincomp.Timer1.Action:
Sub Action()
  if stop=false then
    me.mode=0
  end if
  self.close
End Sub

FTP.comparaarquivos:
Function comparaarquivos(path as string) As boolean
  dim i as integer
  for i=0 to listbox1.listcount-1
    if listbox1.cell(i,6)=path then
      return false
    end if
  next
  return true
End Function

FTP.addfoldertodb:
Sub addfoldertodb(pasta as folderItem)
  dim i as integer
  dim fd as folderItem
  
  for i=1 to pasta.Count
    fd=pasta.TrueItem(i)
    if fd.isreadable then
      if fd.Visible then
        if fd.directory then
          addfoldertodb(fd)
        else
          addfiletodb(fd)
        end if
      end if
    end if
  next
End Sub

FTP.cancelaos:
Sub cancelaos()
  
End Sub

FTP.EnableMenuItems:
Sub EnableMenuItems()
  arquivocancelaos.enable
End Sub

FTP.connectbutton.Action:
Sub Action()
  if me.caption="Conectar" then
    connecttoserver
  else
    comsock.write "QUIT"+chr(10)
    me.caption="Conectar"
    me.default=true
  end if
End Sub

FTP.stopbutton.Action:
Sub Action()
  stop=true
  if datasock<>nil then
    if datasock.isConnected then
      timer1.mode=0
      timer2.mode=0
      progressBar1.value=0
      listbox1.cell(0,2)="Esperando..."
    end if
  end if
  if chasingArrows1.visible=true then
    chasingArrows1.visible=false
  end if
End Sub

FTP.delbutton.Action:
Sub Action()
  dim fd as folderItem
  dim i as integer
  if localhasfocus=true then
    For i=0 to listbox1.Listcount-1
      if listbox1.selected(i) then
        if listbox1.cell(i,3)="yes" then
          fd=getfolderItem(listbox1.cell(i,6))
          removefromdb(i)
          if fd.maccreator="OCTO" and fd.mactype="OCT!" then
            fd.delete
          end if
        elseif listbox1.cell(i,3)="no" then
          fd=getfolderItem(listbox1.cell(i,6))
          removefromdb(i)
        end if
        listbox1.removeRow i
        i=-1
      end if
    next
    refreshqueue
  elseif remotehasfocus=true then
    For i=0 to listBox2.listcount-1
      if listBox2.selected(i) then
        alerta="Deseja remover o arquivo "+ftp.listbox2.cell(i,0)+" do servidor remoto?"
        winalerta.showmodal
        if sim then
          comsock.write "DELE "+listBox2.cell(i,0)+chr(10)
          ftpdb.SQLExecute("delete from files where name='"+ftp.listbox2.cell(i,0)+"'")
          ftpdb.close
          listBox2.removerow i
          i=-1
        end if
      end if
    next
  end if
  me.enabled=false
End Sub

FTP.confbutton.Action:
Sub Action()
  wincon.showWithin self
End Sub

FTP.listbox1.CellBackgroundPaint:
Function CellBackgroundPaint(g As Graphics, row As Integer, column As Integer) As Boolean
  if (row mod 2)=0 then
    g.foreColor=rgb(232,235,255)
    g.fillrect 0,0,g.Width,g.height
  end if
  if me.selected(row) then
    g.foreColor = rgb(66,82,255)
    g.fillrect 0,0,g.Width,g.height
    g.foreColor=rgb(255,255,255)
  else
    g.foreColor=rgb(0,0,0)
  end if
  //g.drawstring me.cell(me.lastIndex,1),0,0
End Function

FTP.listbox1.LostFocus:
Sub LostFocus()
  localhasfocus=false
  delbutton.enabled=false
End Sub

FTP.listbox1.CellClick:
Function CellClick(row as Integer, column as Integer, x as Integer, y as Integer) As Boolean
  localhasfocus=true
  delbutton.enabled=true
End Function

FTP.listbox1.MouseDown:
Function MouseDown(x As Integer, y As Integer) As Boolean
  if y<21 then
    return true
  end if
End Function

FTP.listbox1.KeyDown:
Function KeyDown(key As String) As Boolean
  dim i as integer
  dim fd as folderItem
  if key=chr(8) or key=chr(127) then
    For i=0 to ftp.listbox1.Listcount-1
      if ftp.listbox1.selected(i) then
        if ftp.listbox1.cell(i,3)="yes" then
          fd=getfolderItem(ftp.listbox1.cell(i,6))
          removefromDB(i)
          fd.delete
        elseif ftp.listbox1.cell(i,3)="no" then
          fd=getfolderItem(ftp.listbox1.cell(i,6))
          removefromDB(i)
        end if
        ftp.listbox1.removeRow i
        i=-1
      end if
    next
    refreshqueue
  end if
End Function

FTP.listbox1.Open:
Sub Open()
  me.acceptfileDrop("allfiles")
  'me.Column(0).UserResizable =true
  'me.Column(1).UserResizable =true
  'me.Column(2).UserResizable =true
  'me.Column(3).UserResizable =true
  'me.Column(4).UserResizable =true
  me.columnAlignment(1)=2
  me.columnAlignment(2)=2
  me.columnAlignment(3)=2
  me.columnAlignment(4)=2
  me.columnAlignment(5)=3
  me.ScrollBarVertical=true
  refreshqueue
End Sub

FTP.listbox1.DropObject:
Sub DropObject(obj As DragItem)
  dim cod as string
  dim last as integer
  dim fd as folderItem
  dim pasta as string
  
  Do
    If Obj.FolderItemAvailable then
      fd=obj.folderItem
      cod=right(fd.name,3)
      if comparaarquivos(fd.absolutePath) then
        if fd.Alias=false and fd.isreadable=true then
          if cod="zip" or cod="sit" or cod="oct" or cod="rar" then
            addcompressedfiletodb(fd)
          else
            addfiletodb(fd)
          end if
        end if
      else
        msgBox "O arquivo "+fd.name+" j‡ est‡ agendado."    // <-- CONVERTED
      end if
    end if
  Loop until Not obj.NextItem
  refreshqueue
End Sub

FTP.compress.Action:
Sub Action()
  dim no as boolean
  dim i as integer
  stop=false
  for i=0 to listbox1.listcount-1
    if listbox1.cell(i,3)="no" then
      no=true
    end if
  next
  if no=true then
    wincomp.show
    zip=new OCTcompress
    zip.run
  end if
End Sub

FTP.send.Action:
Sub Action()
  dim yes as boolean
  dim i as integer
  for i=0 to listbox1.listcount-1
    if listbox1.cell(i,3)="no" then
      yes=true
    end if
  next
  if yes=false then
    settransfer
  else
    MsgBox "Os arquivos devem estar compactados para o envio."
  end if
  stop=false
End Sub

FTP.ListBox2.CellBackgroundPaint:
Function CellBackgroundPaint(g As Graphics, row As Integer, column As Integer) As Boolean
  if (row mod 2)=0 then
    g.foreColor=rgb(232,235,255)
    g.fillrect 0,0,g.Width,g.height
  end if
  if me.selected(row) then
    g.foreColor = rgb(66,82,255)
    g.fillrect 0,0,g.Width,g.height
    g.foreColor=rgb(255,255,255)
  else
    g.foreColor=rgb(0,0,0)
  end if
  //g.drawstring me.cell(me.lastIndex,1),0,0
End Function

FTP.ListBox2.KeyDown:
Function KeyDown(key As String) As Boolean
  dim i as integer
  dim fd as folderItem
  if key=chr(8) or key=chr(127) then
    if comsock<>nil then
      if comsock.isConnected then
        For i=0 to ftp.listbox2.Listcount-1
          if ftp.listbox2.selected(i) then
            alerta="Deseja remover o arquivo "+ftp.listbox2.cell(i,0)+" do servidor remoto?"
            winalerta.showmodal
            if sim then
              comsock.write "DELE "+ftp.listbox2.cell(i,0)+chr(10)
              ftpdb.SQLExecute("delete from files where name='"+ftp.listbox2.cell(i,0)+"'")
              ftpdb.close
              ftp.listbox2.removeRow i
              i=-1
            end if
          end if
        next
      end if
    end if
  end if
End Function

FTP.ListBox2.Open:
Sub Open()
  me.scrollbarvertical=true
  me.column(0).UserResizable=true
  me.ColumnAlignment(1)=3
End Sub

FTP.ListBox2.LostFocus:
Sub LostFocus()
  remotehasfocus=false
  delbutton.enabled=false
End Sub

FTP.ListBox2.CellClick:
Function CellClick(row as Integer, column as Integer, x as Integer, y as Integer) As Boolean
  remotehasfocus=true
  delbutton.enabled=true
End Function

FTP.Timer1.Action:
Sub Action()
  dim now as date
  dim lixo as string
  now = new date
  
  if txtvelocidade.visible=false then
    txtvelocidade.visible=true
  end if
  if comsock.append=false then
    datasock.bps = datasock.FileSent/(now.totalSeconds - clock.totalSeconds)
    
    lixo=format((datasock.bps/1000),"###.###\ \K\B\/\s")
    txtvelocidade.text="Velocidade:    "+lixo
  else
    datasock.bps = remotefilesize/(now.totalSeconds - clock.totalSeconds)
    
    txtvelocidade.text="Velocidade:    "+format(datasock.bps/1000,"###.###\ \K\B\/\s")
  end if
End Sub

FTP.Timer2.Action:
Sub Action()
  connecttoserver
End Sub

logwindow.tab:
Function tab(number as integer) As string
  dim s as string
  dim i as integer
  
  if number<=0 then
    return " "
  else
    for i=1 to number
      s=s+" "
    next
    return s
  end if
End Function

logwindow.Close:
Sub Close()
  theend
  restorquit.show
End Sub

logwindow.Open:
Sub Open()
  me.title="Envio de Arquivos da O.S. "+os
End Sub

logwindow.EnableMenuItems:
Sub EnableMenuItems()
  arquivoimprimir.enable
  arquivofechar.enable
End Sub

logwindow.EditField1.Open:
Sub Open()
  dim recset as recordSet
  dim txtos, txtinicio, txtcontato, txttermino, txtemail, txtobs, enter as string
  dim i as integer
  
  if targetcarbon then
    enter=chr(13)
  elseif targetmacOS then
    enter=chr(13)
  elseif targetwin32 then
    enter=chr(10)
  end if
  
  recset=ftpdb.sQLSelect("select * from job where os="+chr(34)+os+chr(34)+" and status="+chr(34)+"new"+chr(34))
  txtos="N¼ da O.S.:   "+recset.field("os").getString    // <-- CONVERTED
  if len(txtos)>42 then
    txtos=left(txtos,42)
  end if
  txtcontato="Contato: "+recset.field("contato").getString
  if len(txtcontato)>42 then
    txtcontato=left(txtcontato,42)
  end if
  txtemail="Email:   "+recset.field("email").getString
  if len(txtemail)>42 then
    txtemail=left(txtemail,42)
  end if
  txtobs="Informa›es Adicionais:    "+recset.field("obs").getString    // <-- CONVERTED
  txtinicio="In’cio:  "+recset.field("inicio").getString    // <-- CONVERTED
  if len(txtinicio)>42 then
    txtinicio=left(txtinicio,42)
  end if
  txttermino="TŽrmino: "+recset.field("termino").getString    // <-- CONVERTED
  if len(txttermino)>42 then
    txttermino=left(txttermino,42)
  end if
  recset.close
  
  me.text=tab((85-len("Relat—rio de Envio de Arquivos"))/2)+"Relat—rio de Envio de Arquivos"+enter+enter    // <-- CONVERTED
  me.text=me.text+"_____________________________________________________________________________________"+enter+enter
  me.text=me.text+txtos+tab(85-len(txtos)-len(txtinicio))+txtinicio+enter
  me.text=me.text+txtcontato+tab(85-len(txtcontato)-len(txttermino))+txttermino+enter
  me.text=me.text+txtemail+enter+enter
  me.text=me.text+txtobs+enter+enter
  me.text=me.text+"_____________________________________________________________________________________"+enter+enter
  me.text=me.text+tab((85-len("Arquivos"))/2)+"Arquivos"+enter+enter
  recset=ftpdb.sQLSelect("select * from files")
  while not recset.eof
    me.text=me.text+recset.field("name").getString+enter
    recset.movenext
  wend
  recset.close
  ftpdb.close
End Sub

logwindow.PushButton1.Action:
Sub Action()
  dim stp as styledTextPrinter
  dim g as graphics
  dim ps as printerSetup
  dim pagewidth as integer
  dim pageheight as integer
  
  ps=new printerSetup
  
  if pagesetup<>"" then
    ps.setupString=pagesetup
    pagewidth=ps.width
    pageheight=ps.height
    g=openprinterdialog(ps)
  else
    g=openPrinterDialog()
    pagewidth=72*7.5
    pageheight=72*9
  end if
  if g<> nil then
    stp=editField1.styledtextPrinter(g,pagewidth-28)
    do until stp.eof
      stp.drawBlock 36,36,pageheight-38
      if not stp.eof then
        g.nextpage
      end if
    loop
  end if
End Sub

winalerta.StaticText1.Open:
Sub Open()
  me.text=alerta
End Sub

winalerta.PushButton1.Action:
Sub Action()
  sim=true
  self.close
End Sub

winalerta.PushButton2.Action:
Sub Action()
  sim=false
  self.close
End Sub

winalerta.Canvas1.Paint:
Sub Paint(g As Graphics)
  g.drawcautionIcon 0,0
End Sub

App.getconsetupfromdatabase:
Sub getconsetupfromdatabase()
  dim cur as recordSet
  
  cur=ftpdb.SQLSelect("Select host,user,passwd,speed from consetup")
  host=cur.field("host").StringValue
  user=cur.field("user").StringValue
  passwd=cur.field("passwd").StringValue
  speed=cur.field("speed").IntegerValue
  cur.close
  ftpdb.close
End Sub

App.Open:
Sub Open()
  
  
  getconsetupfromdatabase
  
  
End Sub

App.EnableMenuItems:
Sub EnableMenuItems()
  filequit.enable
End Sub

CancelSocket.list:
Sub list()
  lsock=new listDataSocket
  lsock.address=host
  lsock.port=portnum
  lsock.connect
End Sub

CancelSocket.pasvportdiscovery:
Sub pasvportdiscovery(byref data as string)
  dim temp, temp1,temp2 as string
  dim i as integer
  data=replaceall(data,chr(10),"")
  data=replaceall(data,chr(13),"")
  i=countFields(data," ")
  temp=nthField(data, " ",i)
  i=countFields(temp,",")
  temp1=nthField(temp,",",i-1)
  temp2=nthField(temp,",",i)
  temp2=left(temp2,len(temp2)-1)
  i=((val(temp1))*256)+(val(temp2))
  portnum=i
End Sub

CancelSocket.erasequeue:
Sub erasequeue()
  dim i as integer
  dim fd as folderItem
  
  for i=0 to ftp.listbox1.listcount-1
    if ftp.listbox1.cell(i,3)<>"no" then
      fd=getfolderItem(ftp.listbox1.cell(i,6))
      fd.delete
    end if
  next 
  ftpdb.sQLExecute("delete from queue")
  ftpdb.commit
  wincancelamento.progressBar1.value=wincancelamento.progressBar1.value+1
End Sub

CancelSocket.finaldocancelamento:
Sub finaldocancelamento()
  wincancelamento.progressBar1.visible=false
  wincancelamento.chasingArrows1.visible=false
  wincancelamento.pushButton1.visible=true
  wincancelamento.staticText1.text="O envio da OS "+os+" foi cancelado e os arquivos excluidos."
End Sub

CancelSocket.eraseremotefiles:
Sub eraseremotefiles()
  dim i as integer
  
  for i=0 to ftp.listbox2.listcount-1
    csocket.write "DELE "+ftp.listbox2.cell(i,0)+chr(10)
  next
  me.write "CWD .."+chr(10)
  me.write "RMD "+directory+chr(10)
  theend
End Sub

CancelSocket.Connected:
Sub Connected()
  wincancelamento.progressBar1.value=wincancelamento.progressBar1.value+1
End Sub

CancelSocket.DataAvailable:
Sub DataAvailable()
  dim cod,data,lixo,temp as string
  dim i,j as integer
  data=me.readall
  data = replaceall(data, chr(13), "")
  
  for i=1 to countFields(data,chr(10))
    lixo=nthField(data,chr(10),i)
    cod = left(lixo, 4)
    select case cod
      
    case "220 " //--------------------------------------------------------------------------------------//
      
      me.write "USER "+user+chr(10)
      wincancelamento.progressBar1.value=wincancelamento.progressBar1.value+1
    case "331 " //--------------------------------------------------------------------------------------//
      
      me.write "PASS "+passwd+chr(10)
      status="login"
      wincancelamento.progressBar1.value=wincancelamento.progressBar1.value+1
    case "230 " //--------------------------------------------------------------------------------------//
      
      if status="login" then
        me.write "CWD "+directory+chr(10)
        status="pasv"
      end if
      wincancelamento.progressBar1.value=wincancelamento.progressBar1.value+1
    case "250 " //--------------------------------------------------------------------------------------//
      
      if status="pasv" then
        me.write "PASV"+chr(10)
        status="list"
      end if
      wincancelamento.progressBar1.value=wincancelamento.progressBar1.value+1
    case "227 " //--------------------------------------------------------------------------------------//
      
      if status="list" then
        pasvPortDiscovery(lixo)
        list
      end if
      wincancelamento.progressBar1.value=wincancelamento.progressBar1.value+1
    case "226 " //--------------------------------------------------------------------------------------//
      
      eraseremotefiles
      erasequeue
      ftp.listbox2.deleteAllRows
      finaldocancelamento
      wincancelamento.progressBar1.value=wincancelamento.progressBar1.value+1
      
    case "550 " //--------------------------------------------------------------------------------------//
      
      erasequeue
      finaldocancelamento
      theend
      wincancelamento.progressBar1.value=wincancelamento.progressBar1.value+4
      
    end select //--------------------------------------------------------------------------------------//
    
  next
  data=""
End Sub

Wincancelamento.PushButton1.Action:
Sub Action()
  self.close
  restorquit.show
End Sub

WinConexao.compare:
Function compare() As string
  dim s,t,x as string
  dim i,j as integer
  
  s = (new SHA1).HMAC(editfield1.text, editfield2.text+"DESENVOLVIDOPORMARCIOKRICHELDORF")
  
  for i = 1 to lenB(s)
    if (j = 4) then
      t = t + " "
    end if
    j = (j mod 4) + 1
    x = hex(ascB(midB(s,i,1)))
    if (lenB(x) = 1) then
      x = "0" + x
    end if
    t = t + x
  next
  return t
End Function

WinConexao.CheckNotFinishedOs:
Function CheckNotFinishedOs() As boolean
  dim cur as recordSet
  cur=new recordSet
  cur=ftpdb.sQLSelect("select * from job where status='new'")
  while not cur.eof
    if cur.field("status").getString="new" then
      directory=cur.field("directory").getString
      os=cur.field("os").getString
      lognumber=cur.field("number").IntegerValue
      return true
    else
      return false
    end if
    cur.movenext
  wend
  cur.close
  ftpdb.close
End Function

WinConexao.checkstatus:
Sub checkstatus()
  dim i as integer
  
  if checknotfinishedos=true then
    msgbox "O envio de arquivos para a O.S. "+os+" est‡ incompleto. Continuando..."    // <-- CONVERTED
    ftp.show
  else
    i=rnd*1000000
    directory=str(i)
    info.show
  end if
  
  if speed=0 then
    speed=7168
  end if
End Sub

WinConexao.Open:
Sub Open()
  octopus=new openbasedatabase
  octopus.DatabaseName="Octopus"
  octopus.Host="10.0.0.139"
  octopus.UserName="user"
  octopus.Password="g4quantic9oct"
  octopus.softwareID="REALbasic"
End Sub

WinConexao.PushButton2.Action:
Sub Action()
  Quit
End Sub

WinConexao.PushButton1.Action:
Sub Action()
  dim recset as recordSet
  
  editfield1.enabled=false
  editfield2.enabled=false
  chasingArrows1.visible=true
  staticText1.text="Estabelecendo uma conex‹o. Agurde por favor."    // <-- CONVERTED
  usuario=editfield1.text
  if octopus.connect then
    recset=new recordSet
    recset=octopus.sqLSelect("Select Usuario, Senha, CodEmpresa, Email, NomeCompleto from clientes_fornecedores_usuarios where Usuario="+chr(34)+usuario+chr(34))
    if recset<>nil then
      if recset.eof then
        staticText1.text="Usu‡rio n‹o cadastrado."    // <-- CONVERTED
      else
        if recset.field("Senha").getString=compare then
          codigodaEmpresa=recset.field("CodEmpresa").getString
          email=recset.field("Email").getString
          NomeCompleto=recset.field("NomeCompleto").getString
          recset.close
          'recset=octopus.sQLSelect("Select NomeFantasia from clientes_fornecedores where Codigo='"+codigodaEmpresa+"'")
          'nomefantasia=recset.field("NomeFantasia").getString
          self.close
          checkstatus
          return
        else
          staticText1.text="Senha Inv‡lida."    // <-- CONVERTED
        end if
      end if
    end if
  else
    staticText1.text="Impossivel estabelecer conex‹o. Verifique as configura›es de rede."    // <-- CONVERTED
  end if
  editfield1.enabled=true
  editfield2.enabled=true
  chasingArrows1.visible=false
End Sub

SHA1.IntToLong:
Protected Function IntToLong(i as integer) As String
  // Implementation by Matthijs van Duin
  #pragma DisableBackgroundTasks
  
  dim s as string
  
  s = chrB(BitwiseAnd(BitwiseAnd(i, &hFF000000) \ &h1000000, &hFF))
  s = s + chrB(BitwiseAnd(i, &h00FF0000) \ &h10000)
  s = s + chrB(BitwiseAnd(i, &h0000FF00) \ &h100)
  s = s + chrB(BitwiseAnd(i, &h000000FF))
  
  return s
End Function

SHA1.Init:
Protected Sub Init(data as string)
  // Implementation by Matthijs van Duin
  #pragma DisableBackgroundTasks
  
  dim s as string
  dim len, i as integer
  
  len = lenB(data)
  blocks = (len + 72) \ 64
  
  if (blocks > mlen) then
    mlen = blocks
    M = NewMemoryBlock(mlen * 64)
  else
    for i = (lenB(data) \ 4) to (blocks * 16 - 1)
      M.long(i*4) = 0
    next
  end if
  
  M.cstring(0) = data
  M.byte(lenB(data)) = &h80
  M.long(blocks * 64 - 4) = len * 8
  
  H0 = &h67452301
  H1 = &hEFCDAB89
  H2 = &h98BADCFE
  H3 = &h10325476
  H4 = &hC3D2E1F0
End Sub

SHA1.Hash:
Function Hash(input as string) As String
  // Implementation by Matthijs van Duin
  #pragma DisableBackgroundTasks
  
  dim s as string
  dim i, j as integer
  
  Init(input)
  
  for i = 0 to (blocks - 1)
    for j = 0 to 63 step 4
      W.long(j) = M.long(i * 64 + j)
    next
    
    DoBlock
  next
  
  return IntToLong(H0) + IntToLong(H1) + IntToLong(H2) + IntToLong(H3) + IntToLong(H4)
End Function

SHA1.f1:
Protected Function f1(b as integer, c as integer, d as integer) As Integer
  // Implementation by Matthijs van Duin
  #pragma DisableBackgroundTasks
  
  return BitwiseAnd(b, c) + BitwiseAnd(BitwiseXor(-1, b), d)
End Function

SHA1.LongToInt:
Protected Function LongToInt(s as string) As Integer
  // Implementation by Matthijs van Duin
  #pragma DisableBackgroundTasks
  
  dim i as integer
  
  i = ascB(midB(s, 1, 1)) * &h1000000 + ascB(midB(s, 2, 1)) * &h10000
  i = i + ascB(midB(s, 3, 1)) * &h100 + ascB(midB(s, 4, 1))
  
  return i
End Function

SHA1.DoBlock:
Protected Sub DoBlock()
  // Implementation by Matthijs van Duin
  #pragma DisableBackgroundTasks
  
  dim i, t as integer
  dim a, b, c, d, e, temp as integer
  
  for t = 64 to 319 step 4
    temp = BitwiseXor(BitwiseXor(W.long(t - 12), W.long(t - 32)), BitwiseXor(W.long(t - 56), W.long(t - 64)))
    if (temp < 0) then
      W.long(t) = temp * 2 + 1
    else
      W.long(t) = temp * 2
    end if
  next
  
  a = H0
  b = H1
  c = H2
  d = H3
  e = H4
  
  for t = 0 to 79 step 4
    temp = S5(a) + f1(b, c, d) + e + W.long(t) + &h5A827999
    e = d
    d = c
    c = S30(b)
    b = a
    a = temp
  next
  for t = 80 to 159 step 4
    temp = S5(a) + f2(b, c, d) + e + W.long(t) + &h6ED9EBA1
    e = d
    d = c
    c = S30(b)
    b = a
    a = temp
  next
  for t = 160 to 239 step 4
    temp = S5(a) + f3(b, c, d) + e + W.long(t) + &h8F1BBCDC
    e = d
    d = c
    c = S30(b)
    b = a
    a = temp
  next
  for t = 240 to 319 step 4
    temp = S5(a) + f4(b, c, d) + e + W.long(t) + &hCA62C1D6
    e = d
    d = c
    c = S30(b)
    b = a
    a = temp
  next
  
  H0 = H0 + a
  H1 = H1 + b
  H2 = H2 + c
  H3 = H3 + d
  H4 = H4 + e
End Sub

SHA1.S5:
Function S5(x as integer) As Integer
  // Implementation by Matthijs van Duin
  #pragma DisableBackgroundTasks
  
  dim temp as integer
  
  if (x < 0) then
    temp = BitwiseXor(-1, x)
    return BitwiseXor(-1, BitwiseOr(temp \ &h08000000, temp * &h00000020))
  else
    return BitwiseOr(x \ &h08000000, x * &h00000020)
  end if
End Function

SHA1.HMAC:
Function HMAC(key as string, data as string) As string
  // Implementation by Matthijs van Duin
  #pragma DisableBackgroundTasks
  
  dim ikey, okey, k as string
  dim temp, i as integer
  
  if (lenB(key) > 64) then
    k = Hash(key) 
  else
    k = key
  end if
  
  for i = 1 to 64
    temp = ascB(midB(k, i, 1))
    ikey = ikey + chrB(BitwiseXor(temp, &h36))
    okey = okey + chrB(BitwiseXor(temp, &h5C))
  next
  
  return Hash(okey + Hash(ikey + data)) 
End Function

SHA1.f2:
Protected Function f2(b as integer, c as integer, d as integer) As integer
  // Implementation by Matthijs van Duin
  #pragma DisableBackgroundTasks
  return BitwiseXor(b, BitwiseXor(c, d))
End Function

SHA1.f3:
Protected Function f3(b as integer, c as integer, d as integer) As integer
  // Implementation by Matthijs van Duin
  #pragma DisableBackgroundTasks
  
  return BitwiseOr(BitwiseAnd(b, c), BitwiseOr(BitwiseAnd(b, d), BitwiseAnd(c, d)))
End Function

SHA1.f4:
Protected Function f4(b as integer, c as integer, d as integer) As integer
  // Implementation by Matthijs van Duin
  #pragma DisableBackgroundTasks
  
  return BitwiseXor(b, BitwiseXor(c, d))
End Function

SHA1.S30:
Protected Function S30(x as integer) As integer
  // Implementation by Matthijs van Duin
  #pragma DisableBackgroundTasks
  
  dim temp as integer
  
  if (x < 0) then
    temp = BitwiseXor(-1, x)
    return BitwiseXor(-1, BitwiseOr(temp \ &h00000004, temp * &h40000000))
  else
    return BitwiseOr(x \ &h00000004, x * &h40000000)
  end if
End Function

SHA1.SHA1:
Protected Sub SHA1()
  W = NewMemoryBlock(320)
End Sub

