Messenger.newchatsession:
Sub newchatsession(user as string, address as string, connect as boolean)
  dim n as integer
  
  n=ubound(chatsocket)
  
  chatsocket(n)=new chatmainSocket
  chatsocket(n).address=address
  chatsocket(n).port=44450
  chatsocket(n).windowindex=n
  chatsocket(n).user=user
  
  
  if connect then
    chatsocket(n).connect
  else
    chatsocket(n).listen
  end if
  
  
  redim chatsocket(n+1)
  
End Sub

Messenger.rearrangeindexes:
Sub rearrangeindexes(index as integer)
  dim i as integer
  if ubound(chatwindow)<>0 then
    chatwindow.remove index
    chatsocket.remove index
    
    for i=index to ubound(chatsocket)
      if chatwindow(i)<>nil and chatsocket(i)<>nil then
        chatwindow(i).socketindex=chatwindow(i).socketindex-1
        chatsocket(i).windowindex=chatsocket(i).windowindex-1
      end if
    next
  end if
End Sub

Messenger.notifystart:
Sub notifystart(text as string)
  // Added 11/25/2001 by Jarvis Badgley
  
  // 68k Compatible
  
  #if TargetMacOS
    
    dim err As integer
    if NotificationHandle=nil then
      NotificationHandle=newMemoryBlock(36)
      NotificationFlag=false
    end
    if NotificationFlag then
      return
    end
    NotificationFlag=true
    
    NotificationHandle.short(4) = 8
    NotificationHandle.short(14) = 1
    NotificationHandle.long(16) = 0
    NotificationHandle.long(20) = 0
    NotificationHandle.long(28) = 0
    NotificationHandle.long(32) = 0
    
    if text = "" then
      NotificationHandle.long(24) = 0
    else
      Notify_messageHolder = NewMemoryBlock(len(text) + 1)
      Notify_messageHolder.pstring(0) = text
      
      NotificationHandle.ptr(24) = Notify_messageHolder
    end if
    
    #if TargetCarbon then
      Declare Function NMInstall Lib "CarbonLib" (nmReqPtr as Ptr) as Short
    #else
      Declare Function NMInstall Lib "InterfaceLib" (nmReqPtr as Ptr) as Short Inline68K("205FA05E3E80")
    #endif
    
    err= NMInstall(NotificationHandle)
    
  #endif
  
End Sub

Messenger.notifystop:
Sub notifystop()
  // Added 11/25/2001 by Jarvis Badgley
  
  // 68k Compatible
  
  #if TargetMacOS
    
    dim err as integer
    if NotificationHandle<>nil and NotificationFlag then
      
      #if TargetCarbon then
        Declare Function NMRemove Lib "CarbonLib" (nmReqPtr as Ptr) as Short Inline68K("205FA05F3E80")
      #else
        Declare Function NMRemove Lib "InterfaceLib" (nmReqPtr as Ptr) as Short Inline68K("205FA05F3E80")
      #endif
      
      err= NMRemove(NotificationHandle)
    end
    NotificationFlag=false
    
  #endif
End Sub

Messenger.disablestatus:
Sub disablestatus()
  status.pushButton1.enabled=false
  status.menuteclarcom.enabled=false
  status.listBox1.enabled=false
End Sub

Messenger.enablestatus:
Sub enablestatus()
  status.pushButton1.enabled=true
  status.menuteclarcom.enabled=true
  status.listBox1.enabled=true
End Sub

ChatMainSocket.Error:
Sub Error()
  dim alert as alerta
  
  select case me.lastErrorCode
  case 102
    me.close
    if chatwindow(windowindex)<>nil then
      chatwindow(windowindex).close
    end if
    rearrangeindexes(windowindex)
    #if targetcarbon then
      alertmessage=user+" desconectou do chat."
      alert=new alerta
      alert.show
    #else
      msgBox user+" desconectou do chat."
    #endif
  case 101
    #if targetcarbon then
      alertmessage=user+" est‡ Off-Line. Tente mais tarde."    // <-- CONVERTED
      alert=new alerta
      alert.show
    #else
      msgBox user+" est‡ Off-Line. Tente mais tarde."    // <-- CONVERTED
    #endif
    me.close
    conectando.close
    rearrangeindexes(windowindex)
  case -3247
    me.connect
  else
    msgBox "chat socket "+str(me.lastErrorCode)
  end select
End Sub

ChatMainSocket.DataAvailable:
Sub DataAvailable()
  dim data as string
  data=me.readall
  chatwindow(me.windowindex).editField1.SelStart=Len(chatwindow(me.windowindex).editField1.Text)
  chatwindow(me.windowindex).editField1.seltextcolor=rgb(20,20,255)
  chatwindow(me.windowindex).editField1.seltext=data+chr(10)
  chatwindow(me.windowindex).editField1.seltextcolor=rgb(0,0,0)
End Sub

ChatMainSocket.Connected:
Sub Connected()
  chatwindow(windowindex)=new chat
  chatwindow(windowindex).socketindex=windowindex
  chatwindow(windowindex).title="Teclando com "+user
  chatwindow(windowindex).show
  redim chatwindow(windowindex+1)
End Sub

app.Open:
Sub Open()
  colaborador="LEONARDO"
  codEmpresa="1"
  wintools.entradasdechat.listen
End Sub

chat.send:
Sub send()
  chatsocket(socketindex).write app.colaborador+": "+editfield2.text
  editfield1.seltext=app.colaborador+": "+editfield2.text+chr(10)
  editfield2.text=""
End Sub

chat.Close:
Sub Close()
  chatsocket(socketindex).close
End Sub

chat.Open:
Sub Open()
  dim i as integer
  dim a as string
  me.height=status.height
  me.left=screen(0).width-me.width-status.width-1
  me.top=38
End Sub

chat.EditField2.KeyDown:
Function KeyDown(Key As String) As Boolean
  if asc(key)=13 then
    if me.text<>"" then
      send
    end if
    return true
  elseif asc(key)>=10 and asc(key)<=127 then
    return false
  elseif asc(key)=9 then
    me.text=me.text+"     "
    return true
  elseif asc(key)>=1 and asc(key)<=8 then
    return false
  else
    return true
  end if
End Function

chat.BevelButton1.Action:
Sub Action()
  send
End Sub

Status.populatemenuteclarcom:
Sub populatemenuteclarcom()
  dim nome as string
  dim cur as recordSet
  
  menuteclarcom.addrow "Teclar com:"
  cur=Server.sQLSelect("select Colaborador, IPouEstacao from estacoes_log where status='CONECTADO'")
  menuteclarcom.addseparator
  while not cur.eOF
    nome=cur.field("Colaborador").getString
    if nome<>app.colaborador then
      menuteclarcom.addrow nome
      menuteclarcom.rowTag(menuteclarcom.Listcount-1)=cur.field("IPouEstacao").getString
    end if
    cur.movenext
  wend
  cur.close
  menuteclarcom.listindex=0
End Sub

Status.checknewmessages:
Sub checknewmessages()
  dim r as recordSet
  
  r=octopus.sQLSelect("select De,Data,_rowid,Nova from chat where CodEmpresa="+chr(34)+app.CodEmpresa+chr(34)+" and Usuario="+chr(34)+app.colaborador+chr(34))
  
  listBox1.deleteAllRows
  if r<>nil then
    while not r.eof
      listBox1.addrow r.field("De").getString
      listbox1.cell(listBox1.lastIndex,1)=r.field("Data").getString
      listbox1.cell(listBox1.lastIndex,2)=format(r.field("_rowid").DoubleValue, "###########")
      r.edit
      r.field("Nova").setString("NAO")
      r.Update
      r.movenext
    wend
    r.close
  end if
  octopus.commit
End Sub

Status.Close:
Sub Close()
  statusactive=false
  wintools.timer1.mode=2
End Sub

Status.Open:
Sub Open()
  me.left=screen(0).width-me.width
  me.top=38
  statusactive=true
  wintools.timer1.mode=0
End Sub

Status.menuteclarcom.Open:
Sub Open()
  populatemenuteclarcom
End Sub

Status.menuteclarcom.Change:
Sub Change()
  if me.rowTag(me.listindex)<>nil then
    wintools.saidasdechat.address=me.rowTag(me.listindex)
    wintools.saidasdechat.connect
    alertmessage="Chamando "+me.text+" para chat."
    conectando.show
  end if
  me.listindex=0
End Sub

Status.ListBox1.KeyDown:
Function KeyDown(key As String) As Boolean
  dim i as integer
  i=asc(key)
  
  if i=127 or i=8 then
    for i=0 to me.listcount -1
      if me.Selected(i) then
        octopus.sQLExecute("delete from chat where _rowid="+chr(34)+me.cell(i,2)+chr(34))
        octopus.commit
      end if
    next
  end if
  me.deleteAllRows
  checknewmessages
End Function

Status.ListBox1.DoubleClick:
Sub DoubleClick()
  dim i as integer
  
  for i=0 to me.listcount -1
    if me.Selected(i) then
      message.show
      message.id=me.cell(i,2)
      message.responder.visible=true
      message.responder.enabled=true
      message.readmessage
      me.selected(i)=false
    end if
  next
End Sub

Status.ListBox1.CellBackgroundPaint:
Function CellBackgroundPaint(g As Graphics, row As Integer, column As Integer) As Boolean
  if(row mod 2)=0 then
    g.foreColor = rgb(232,235,255)
    g.fillrect 0,0,g.Width,g.height
    g.drawstring me.cell(me.lastIndex,1),0,0
  end if
  if me.selected(row) then
    g.foreColor = rgb(66,82,255)
    g.fillrect 0,0,g.width,g.height
    g.foreColor = rgb(255,255,255)
  else
    g.foreColor = rgb(0,0,0)
  end if
End Function

Status.ListBox1.Open:
Sub Open()
  checknewmessages
End Sub

Status.PushButton1.Action:
Sub Action()
  message.show
  message.enviar.visible=true
  message.enviar.enabled=true
  message.menumenpara.enabled=true
  message.menumenpara.visible=true
End Sub

Status.Timer1.Action:
Sub Action()
  listBox1.deleteAllRows
  checknewmessages
  menuteclarcom.deleteAllRows
  populatemenuteclarcom
End Sub

Message.populatemenumenpara:
Sub populatemenumenpara()
  dim cur,rset as recordSet
  dim empresa,codigo as string
  menumenpara.addrow "Para"
  menumenpara.addseparator
  
  cur=octopus.sQLSelect("select NomeFantasia from empresa")
  empresa=cur.field("NomeFantasia").getString
  cur.close
  menumenpara.addrow "__ "+empresa+" __"
  
  
  
  cur=octopus.sQLSelect("select usuario from usuarios")
  
  while not cur.eOF
    menumenpara.addrow cur.field("usuario").getString
    menumenpara.rowTag(menumenpara.listcount-1)=app.codEmpresa
    cur.movenext
  wend
  cur.close
  
  menumenpara.addseparator
  cur=octopus.sQLSelect("select Codigo,NomeFantasia from clientes_fornecedores")
  while not cur.eof
    codigo=cur.field("Codigo").getString
    empresa=cur.field("NomeFantasia").getString
    
    rset=octopus.sqLSelect("select usuario from clientes_fornecedores_usuarios where CodEmpresa="+chr(34)+codigo+chr(34))
    if not rset.eof then
      menumenpara.addrow "__ "+empresa+" __"
      while not rset.eof
        menumenpara.addrow rset.field("usuario").getString
        menumenpara.rowTag(menumenpara.listcount-1)=Codigo
        rset.movenext
      wend
      rset.close
      menumenpara.addseparator
    end if
    cur.movenext
  wend
  cur.close
  menumenpara.listindex=0
End Sub

Message.readmessage:
Sub readmessage()
  dim r as recordSet
  
  
  r=octopus.sQLSelect("select Assunto, De, Data, Mensagem from chat where _rowid="+chr(34)+id+chr(34))
  if r<>nil then
    de.visible=true
    de.enabled=true
    de.text="De: "+r.field("De").getString
    data.visible=true
    data.enabled=true
    data.text="Em: "+r.field("Data").getString
    editfield1.text=r.field("Assunto").getString
    editfield2.text=r.field("Mensagem").getString
  end if
  r.close
End Sub

Message.sendmessage:
Sub sendmessage()
  dim rset as recordSet
  dim rec as databaserecord
  dim cod,empresadoremetente as string
  dim now as date
  
  if user="" then
    msgBox "Informe o destinat‡rio."    // <-- CONVERTED
  else
    now=new date
    sending.show
    
    rset=octopus.sQLSelect("select NomeFantasia from empresa")
    empresadoremetente=rset.field("NomeFantasia").getString
    
    rec=new databaserecord
    rec.column("Usuario")=user
    rec.column("CodEmpresa")=codigo
    rec.column("Assunto")=editfield1.text
    rec.column("Mensagem")=editfield2.text
    rec.column("Data")=now.ShortDate+" "+now.shorttime
    rec.column("De")=app.colaborador+"@"+empresadoremetente
    rec.column("Nova")="SIM"
    octopus.insertRecord("chat", rec)
    octopus.commit
    
    self.close
    
    sending.close
  end if
End Sub

Message.deletemessage:
Sub deletemessage()
  octopus.sQLExecute("delete from chat where _rowid="+chr(34)+id+chr(34))
  octopus.commit
  status.listBox1.deleteAllRows
  status.checknewmessages
End Sub

Message.Close:
Sub Close()
  enablestatus
  deletemessage
End Sub

Message.Open:
Sub Open()
  me.height=status.height
  me.left=screen(0).width-me.width-status.width-1
  me.top=38
  me.title="Mensagem para "+user
  disablestatus
End Sub

Message.enviar.Action:
Sub Action()
  sendmessage
End Sub

Message.PushButton2.Action:
Sub Action()
  self.close
End Sub

Message.responder.Action:
Sub Action()
  if de.visible then
    de.visible=false
    data.visible=false
    menumenpara.visible=true
    menumenpara.enabled=true
    me.default=true
    editfield2.text=chr(10)+chr(10)+chr(10)+chr(10)+"Em "+replaceall(data.text,"Em: ","")+" "+replaceall(nthfield(de.text,"@",1),"De: ","")+" escreveu:"+chr(10)+"-----------------------------------"+chr(10)+editfield2.text+chr(10)+"-----------------------------------"
    editfield2.scrollposition=0
    editfield2.SetFocus
  else
    sendmessage
  end if
End Sub

Message.menumenpara.Open:
Sub Open()
  populatemenumenpara
End Sub

Message.menumenpara.Change:
Sub Change()
  dim i as integer
  
  for i=0 to me.listcount-1
    if me.list(i)=me.text then
      if me.rowTag(i)<>nil then
        user=me.text
        codigo=me.rowTag(me.listindex)
      end if
    end if
  next
End Sub

sending.Timer1.Action:
Sub Action()
  select case staticText1.caption
  case "Enviando"
    staticText1.caption="Enviando."
  case "Enviando."
    staticText1.caption="Enviando.."
  case "Enviando.."
    staticText1.caption="Enviando..."
  case "Enviando..."
    staticText1.caption="Enviando"
  end select
End Sub

wintools.saidasdechat.Error:
Sub Error()
  select case me.lastErrorCode
  case 101
    conectando.close
    msgBox "O usu‡rio chamado n‹o est‡ dispon’vel."    // <-- CONVERTED
  case 103
    conectando.close
    msgBox "O usu‡rio chamado n‹o est‡ dispon’vel."    // <-- CONVERTED
  else
    msgBox str(me.lastErrorCode)
  end select
End Sub

wintools.saidasdechat.DataAvailable:
Sub DataAvailable()
  dim data,user as string
  
  
  data=me.readall
  
  user=nthField(data,chr(9),2)
  data=nthField(data,chr(9),1)
  conectando.close
  if data="SIM" then
    newchatsession(user,me.address,true)
  elseif data="NAO" then
    msgBox user+" recusou seu pedido."
  end if
  me.close
  
End Sub

wintools.saidasdechat.Connected:
Sub Connected()
  me.write me.localAddress+chr(9)+app.colaborador
End Sub

wintools.entradasdechat.Error:
Sub Error()
  me.listen
End Sub

wintools.entradasdechat.DataAvailable:
Sub DataAvailable()
  dim data,ip,user as string
  
  data=me.readall
  ip=nthField(data,chr(9),1)
  user=nthField(data,chr(9),2)
  chat_confirmacao.ip=ip
  chat_confirmacao.user=user
  chat_confirmacao.staticText1.text=user+" quer teclar com voc. Aceita?"    // <-- CONVERTED
  chat_confirmacao.visible=true
End Sub

wintools.Timer1.Action:
Sub Action()
  dim r as recordSet
  dim a as alerta
  
  if not statusactive then
    r=octopus.sQLSelect("select De,Data,_rowid,Nova from chat where CodEmpresa="+chr(34)+app.CodEmpresa+chr(34)+" and Usuario="+chr(34)+app.colaborador+chr(34))
    
    if r<>nil then
      if not r.eof then
        while not r.eof 
          if r.field("Nova").getString="SIM" then
            newmessage=true
            //alertmessage="H‡ mensagem para voc!"    // <-- CONVERTED
            //a=new alerta
            //a.show
            beep
            r.movelast
          end if
          r.movenext
        wend
      end if
      r.close
    end if
  end if
End Sub

wintools.BevelButton1.Action:
Sub Action()
  status.show
End Sub

chat_confirmacao.PushButton1.Action:
Sub Action()
  wintools.entradasdechat.write "SIM"+chr(9)+app.colaborador
  newchatsession(user,ip,false)
  self.close
End Sub

chat_confirmacao.PushButton2.Action:
Sub Action()
  self.close
  wintools.entradasdechat.write "NAO"+chr(9)+app.colaborador
End Sub

alerta.Close:
Sub Close()
  notifystop
End Sub

alerta.Open:
Sub Open()
  notifyStart ""
End Sub

alerta.StaticText1.Open:
Sub Open()
  me.text=alertmessage
End Sub

alerta.PushButton1.Action:
Sub Action()
  self.close
  if newmessage then
    status.show
    newmessage=false
  end if
End Sub

conectando.Close:
Sub Close()
  enablestatus
End Sub

conectando.Open:
Sub Open()
  disablestatus
End Sub

conectando.StaticText1.Open:
Sub Open()
  me.text=alertmessage
End Sub

