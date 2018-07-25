Winview.Close:
Sub Close()
  winpalheta.groupBox1.enabled=false
  winpalheta.popupMenu1.listindex=0
End Sub

Winview.SliderZoom.ValueChanged:
Sub ValueChanged()
  TestCanvas.SetZoom(Me.Value)
End Sub

Winview.testCanvas.MouseDrag:
Sub MouseDrag(X as integer, Y as integer)
  'if endx<>x then
  'de_hScrollBar.value=de_hScrollBar.value-(x-startx)
  'startx=x
  'endx=x
  'end if
  '
  'if endy<>y then
  'de_vScrollBar.value=de_vScrollBar.value-(y-starty)
  'starty=y
  'endy=y
  'end if
  'me.drawPicture
  
  
  
End Sub

Winview.de_zoomMenu.OtherZoom:
Sub OtherZoom()
  WinOtherZoom.Show
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
    recset=octopus.sqLSelect("Select Usuario, Senha, CodEmpresa from clientes_fornecedores_usuarios where Usuario="+chr(34)+usuario+chr(34))
    if recset<>nil then
      if recset.eof then
        staticText1.text="Usu‡rio n‹o cadastrado."    // <-- CONVERTED
      else
        if recset.field("Senha").getString=compare then
          codigodaEmpresa=recset.field("CodEmpresa").getString
          recset.close
          recset=octopus.sQLSelect("Select NomeFantasia from clientes_fornecedores where Codigo='"+codigodaEmpresa+"'")
          nomefantasia=recset.field("NomeFantasia").getString
          self.close
          winpalheta.show
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

WinAprovado.PushCancelar.Action:
Sub Action()
  
  radiosfalse
  Close
  
End Sub

WinAprovado.PushOk.Action:
Sub Action()
  dim r as recordSet
  dim rec as databaseRecord
  dim rowid, CodTrab, nome, tipo, tamanho, criado, modificado  as string
  dim now as date
  
  rowid=winpalheta.popupMenu1.rowTag(winpalheta.popupMenu1.listindex)
  rowid=nthField(rowid,"/",2)
  r=octopus.sqLSelect("Select CodTrabalho, NomeDoArquivo, Tipo, Tamanho, Criado, Modificado, Status, _rowid, CodColaborador, CodTarefa from arquivos where _rowid='"+rowid+"'")
  
  codtrab=r.field("CodTrabalho").getString
  nome=r.field("NomeDoArquivo").getString
  tipo=r.field("Tipo").getString
  tamanho=r.field("Tamanho").getString
  modificado=r.field("Modificado").getString
  criado=r.field("Criado").getString
  
  r.edit
  r.field("Status").setString("1")
  r.field("CodColaborador").setString("")
  r.field("CodTarefa").setString("")
  r.update
  octopus.commit
  
  now=new date
  
  rec=new databaserecord
  rec.column("Observacoes")="ARQUIVO APROVADO"
  rec.column("Atividade")="RETORNO DE PROVA REMOTA"
  rec.column("CodTrabalho")=codTrab
  rec.column("NomeDoArquivo")=nome
  rec.column("Colaborador")=nomefantasia+" - "+usuario
  rec.column("Tipo")=tipo
  rec.column("Tamanho")=tamanho
  rec.column("Criado")=criado
  rec.column("Modificado")=modificado
  rec.column("DtAtividade")=now.ShortDate + " " + now.LongTime
  rec.column("CodArquivo")=rowid
  rec.column("CodColaborador")=codigodaEmpresa
  octopus.insertRecord("historico_do_arquivo", rec)
  octopus.commit
  
  winpalheta.popupMenu1.removeRow winpalheta.popupMenu1.listindex
  winview.close
  self.close
  radiosfalse
  winpalheta.groupBox1.enabled=false
  winpalheta.popupMenu1.listindex=0
End Sub

alterarenviarnovaprova.PushOk.Action:
Sub Action()
  dim r as recordSet
  dim rec as databaseRecord
  dim rowid, CodTrab, nome, tipo, tamanho, criado, modificado  as string
  dim now as date
  
  rowid=winpalheta.popupMenu1.rowTag(winpalheta.popupMenu1.listindex)
  rowid=nthField(rowid,"/",2)
  r=octopus.sqLSelect("Select CodTrabalho, NomeDoArquivo, Tipo, Tamanho, Criado, Modificado, Status, _rowid, CodColaborador, CodTarefa from arquivos where _rowid='"+rowid+"'")
  
  codtrab=r.field("CodTrabalho").getString
  nome=r.field("NomeDoArquivo").getString
  tipo=r.field("Tipo").getString
  tamanho=r.field("Tamanho").getString
  modificado=r.field("Modificado").getString
  criado=r.field("Criado").getString
  
  r.edit
  r.field("Status").setString("1")
  r.field("CodColaborador").setString("")
  r.field("CodTarefa").setString("")
  r.update
  octopus.commit
  
  now=new date
  
  rec=new databaserecord
  rec.column("Observacoes")="ALTERAR E ENVIAR NOVA PROVA:   "+editfield1.text
  rec.column("Atividade")="RETORNO DE PROVA REMOTA"
  rec.column("CodTrabalho")=codTrab
  rec.column("NomeDoArquivo")=nome
  rec.column("Colaborador")=nomefantasia+" - "+usuario
  rec.column("Tipo")=tipo
  rec.column("Tamanho")=tamanho
  rec.column("Criado")=criado
  rec.column("Modificado")=modificado
  rec.column("DtAtividade")=now.ShortDate + " " + now.LongTime
  rec.column("CodArquivo")=rowid
  rec.column("CodColaborador")=codigodaEmpresa
  octopus.insertRecord("historico_do_arquivo", rec)
  octopus.commit
  
  winpalheta.popupMenu1.removeRow winpalheta.popupMenu1.listindex
  winview.close
  self.close
  radiosfalse
  winpalheta.groupBox1.enabled=false
  winpalheta.popupMenu1.listindex=0
End Sub

alterarenviarnovaprova.PushCancelar.Action:
Sub Action()
  Close
  radiosfalse
End Sub

Winpalheta.Open:
Sub Open()
  me.width=screen(0).width-16
  me.left=16
  me.top=22
End Sub

Winpalheta.RadioAprovado.Action:
Sub Action()
  winaprovado.show
End Sub

Winpalheta.RadioAprovadoComAlteracao.Action:
Sub Action()
  aprovadocomalteracoes.show
End Sub

Winpalheta.RadioAlterar.Action:
Sub Action()
  alterarenviarnovaprova.show
End Sub

Winpalheta.PopupMenu1.Open:
Sub Open()
  dim r,t as recordSet
  me.addseparator
  me.addseparator
  
  t=octopus.sQLSelect("select Codigo, Descricao from trabalhos where DescricaoStatus='ATIVO' and CodCliente='"+codigodaEmpresa+"'")
  if t<>nil then
    if t.eof then
      msgBox "N‹o existem trabalhos ativos para este cliente"    // <-- CONVERTED
    else
      while not t.eof
        me.addrow "***    "+t.field("Descricao").getString+"    ***"
        me.addseparator
        r=octopus.sQLSelect("select NomeDoArquivo, Imagem, _rowid from arquivos where Imagem<>NULL and Status='0' and CodTarefa='EM PROVA' and CodTrabalho='"+t.field("Codigo").getString+"'")
        if r<>nil then
          if r.eof then
            me.addrow "Sem arquivos para aprova‹o neste momento."    // <-- CONVERTED
          else
            while not r.eof
              me.addrow r.field("NomeDoArquivo").getString
              me.rowTag(me.listCount-1)=r.field("Imagem").getString+"/"+r.field("_rowid").getString
              r.movenext
            wend
          end if
        end if
        me.addseparator
        me.addseparator
        t.movenext
      wend
    end if
  end if
  if r<>nil and t<>nil then
    r.close
    t.close
  end if
End Sub

Winpalheta.PopupMenu1.Change:
Sub Change()
  dim s as string
  dim rowid as string
  dim p as picture
  dim fd as folderItem
  
  if me.rowTag(me.listIndex)<>nil then
    s=me.rowTag(me.listIndex)
    s=nthField(s,"/",1)
    rowid=nthField(s,"/",1)
    fd=getfolderItem("")
    fd=fd.temporaryFolder.child(rowid)
    if fd.Exists then
      p=fd.openAsPicture
    else
      winaguarde.show
      s=octopus.retrieveblob(s)
      p=stringtoPicture(s,rowid,"")
      fd.saveAsPicture p
      winaguarde.close
    end if
    winview.top=90
    winview.width=screen(0).width-30
    winview.left=15
    
    pic=p
    winview.testCanvas.setLockEdges true
    winview.testCanvas.hasZoomMenu=true
    winview.testCanvas.initialize
    winview.staticText1.text=me.text
    groupBox1.enabled=true
  else 
    me.listindex=0
  end if
End Sub

WinOtherZoom.PushOk.Action:
Sub Action()
  winview.TestCanvas.SetZoom(Val(CZoom.Text))
  Close
End Sub

WinOtherZoom.PushCancelar.Action:
Sub Action()
  Close
End Sub

aprovadocomalteracoes.EnableMenuItems:
Sub EnableMenuItems()
  arquivoImprimir.enable
End Sub

aprovadocomalteracoes.PushOk.Action:
Sub Action()
  dim r as recordSet
  dim rec as databaseRecord
  dim rowid, CodTrab, nome, tipo, tamanho, criado, modificado  as string
  dim now as date
  
  rowid=winpalheta.popupMenu1.rowTag(winpalheta.popupMenu1.listindex)
  rowid=nthField(rowid,"/",2)
  r=octopus.sqLSelect("Select CodTrabalho, NomeDoArquivo, Tipo, Tamanho, Criado, Modificado, Status, _rowid, CodColaborador, CodTarefa from arquivos where _rowid='"+rowid+"'")
  
  codtrab=r.field("CodTrabalho").getString
  nome=r.field("NomeDoArquivo").getString
  tipo=r.field("Tipo").getString
  tamanho=r.field("Tamanho").getString
  modificado=r.field("Modificado").getString
  criado=r.field("Criado").getString
  
  r.edit
  r.field("Status").setString("1")
  r.field("CodColaborador").setString("")
  r.field("CodTarefa").setString("")
  r.update
  octopus.commit
  
  now=new date
  
  rec=new databaserecord
  rec.column("Observacoes")="APROVADO COM ALTERA‚ÍES:   "+editfield1.text    // <-- CONVERTED
  rec.column("Atividade")="RETORNO DE PROVA REMOTA"
  rec.column("CodTrabalho")=codTrab
  rec.column("NomeDoArquivo")=nome
  rec.column("Colaborador")=nomefantasia+" - "+usuario
  rec.column("Tipo")=tipo
  rec.column("Tamanho")=tamanho
  rec.column("Criado")=criado
  rec.column("Modificado")=modificado
  rec.column("DtAtividade")=now.ShortDate + " " + now.LongTime
  rec.column("CodArquivo")=rowid
  rec.column("CodColaborador")=codigodaEmpresa
  octopus.insertRecord("historico_do_arquivo", rec)
  octopus.commit
  
  winpalheta.popupMenu1.removeRow winpalheta.popupMenu1.listindex
  winview.close
  self.close
  radiosfalse
  winpalheta.groupBox1.enabled=false
  winpalheta.popupMenu1.listindex=0
End Sub

aprovadocomalteracoes.PushCancelar.Action:
Sub Action()
  Close
  radiosfalse
End Sub

c_displayEngine.setLockEdges:
Sub setLockEdges(b as boolean)
  'Private method simply to lock or unlock the edges of the display engine
  'according to the passed boolean.
  
  self.lockLeft = b
  self.lockRight = b
  self.lockTop = b
  self.lockBottom = b
  
  me.lockEdges = b
End Sub

c_displayEngine.findScrollBars:
Protected Sub findScrollBars()
  dim ct as integer
  dim i as integer
  dim sb as control
  
  
  'One of the first things the displayEngine needs to do is figure out where it's scroll bars are
  'in the window and set them to the appropriate sizes and locations.
  
  'This is a private method because it needs only be run when the displayEngine is first created.
  
  
  'First, figure out how many controls are in the window.
  
  ct = me.window.controlCount
  
  
  'Step through each of the controls and see if any of them are what we are looking for.
  'Specifically: scrollbars with their supers set to "c_de_scrollBar"
  
  for i = 0 to ct - 1
    
    'Get a temporary reference to the control at the current index.
    
    sb =  me.window.control( i )
    
    
    'Check to see if it is a "c_de_scrollBar"
    
    if sb isa c_de_scrollBar then
      
      'It is, so we need to see if we need any more "c_de_scrollBar"s
      
      
      if hScrollBar = nil then
        
        'We don't have a horizontal scrollbar yet, so let's use this scrollbar for that purpose.
        'We need to set up a cross reference so that each control can see each other when needed.
        
        hScrollBar = c_de_scrollBar( sb )
        hScrollBar.parent = self
        
      else
        
        'We already have a horizontal scrollbar, so let's check to see if we need a vertical scrollbar.
        
        if vScrollBar = nil then
          
          'We don't have a vertical scrollbar yet, so let's use this scrollbar for that purpose.
          'We need to set up a cross reference so that each control can see each other when needed.
          
          vScrollBar = c_de_scrollBar( sb )
          vScrollBar.parent = self
          
        end if
      end if
    end if
  next
  
  
  'Okay, we've found the scrollbars. Now, it's time to size and position the scrollbars appropriatly
  'so they appear to be a part of the displayEngine.
  
  'Note that there is not any error-checking yet in this method, although there should be.
  
  
  'Start with the horizontal scrollbar first (the one along the bottom of the displayEngine).
  
  hScrollBar.height = 16     'Set the scrollbar's height.
  
  if hasZoomMenu then
    
    'The displayEngine will be using a zoomMenu, so the scrollbar's width needs to be adjusted
    'accordingly to leave room for the zoomMenu.
    
    hScrollBar.width = me.width - 78
    
  else
    
    'No zoomMenu, so the scrollbar can span the width of the displayEngine.
    
    hScrollBar.width = me.width + 1
  end if
  
  
  hScrollBar.top = me.top + me.height   'Set the scrollbar so it is positioned directly below the
                                                            'display engine's canvas.
  
  
  if hasZoomMenu then
    
    'The displayEngine will be using a zoomMenu, so position the scrollbar appropriately to
    'leave room.
    
    hScrollBar.left = me.left + 79
    
  else
    
    'No zoomMenu, so the scrollbar can be positioned all the way to the left of the displayEngine.
    
    hScrollBar.left = me.left - 1
    
  end if
  
  
  'Now, set the horizontal scrollbar's lock properties so that, even if the window is resized,
  'it will still be in the correct position and be the correct size for the new size of the 
  'display engine.
  
  'Changed to me.lockEdges to allow for having a cDisplayEngine that doesn't resize with window.
  'Scott Crick 09/25/00
  
  hScrollBar.lockLeft = me.lockEdges
  hScrollBar.lockRight = me.lockEdges
  hScrollBar.lockBottom = me.lockEdges
  
  
  'Now move on to the vertical scrollbar, which is simpler to set up than the horizontal scrollbar
  'because we don't need to deal with the zoomMenu for this one. Similar properties are set
  'for a similar effect as the horizontal scrollbar.
  
  vScrollBar.width = 16
  vScrollBar.height = me.height + 2
  vScrollBar.top = me.top - 1
  vScrollBar.left = me.left + me.width
  
  vScrollBar.lockTop = me.lockEdges
  vScrollBar.lockBottom = me.lockEdges
  vScrollBar.lockRight = me.lockEdges
End Sub

c_displayEngine.drawPicture:
Sub drawPicture()
  dim g as graphics
  
  
  'This is the method which handles the drawing of the picture in the displayEngine itself.
  'First it creates an off-screen picture into which it draws the background color and then
  'the picture at the appropriate position, and zoom value.
  
  'beep 'good test to see how often we are updating
  g = me.graphics
  
  
  enableScrollBars     'We need to update the scrollBars first so that we can determine the scroll positions
                                   'for drawing the picture.
  
  
  'Create an off-screen picture so that we can do all our drawing here first to hide any
  'flicker from the user.
  
  p = newPicture( me.width, me.height, screen( 0 ).depth )
  
  if not( p = nil ) then
    
    'The offscreen picture was created successfully, so let's proceed with our drawing tasks.
    
    p.graphics.foreColor = backgroundColor
    p.graphics.fillRect 0, 0, me.width, me.height
    
    
    'The next two calls determine where exactly we're going to be drawing this picture
    'and at what size it will be displayed at.
    
    getDisplaySizes
    getDisplayPosition
    
    
    'Now that we know that, let's draw the picture itself into the off-screen picture
    
    p.graphics.drawPicture pic, disp_left, disp_top, disp_width, disp_height, scrollXPosition, scrollYPosition, picWidth, picHeight
    
    
    'And finally, draw the picture into the engine's canvas.
    g.drawPicture p, 0, 0
    
  end if
End Sub

c_displayEngine.initialize:
Sub initialize()
  dim winWidth as integer
  dim winHeight as integer
  
  'Next two lines added by Scott Crick
  '09/25/00
  'Allows hasZoomMenu to be set at runtime and still allows updates
  'of the zoom menu and scrollbars.
  
  findZoomMenu
  findScrollBars
  
  'This method is called from user code (usually the Open event) so that the displayEngine is ready for use
  'immediately upon being displayed. This method sets sizes, determines window size and also determines
  'how much more "not-displayEngine" space there is in the parent window.
  
  
  'Store values for the picture size to be used in other methods.
  
  picWidth = pic.width
  picHeight = pic.height
  
  
  'Temporary variables to store window size.
  
  winWidth = me.window.width
  winHeight = me.window.height
  
  
  'Figure out how much "not-displayEngine" space there is in the parent window and store it
  'for use by other methods.
  
  dx_width = winWidth - me.width
  dy_height = winHeight - me.height
  
  winWidth = picWidth + dx_width
  winHeight = picHeight + dy_height
  
  findFrameRect
  
  'If autoOpenResize is true, then we need to call the zoomToScreen method. The code from this point
  'forward will probably be updated to allow for a bit more flexibility and is merely here to
  'provide a functional displayEngine.
  
  if autoOpenResize then
    
    zoomToScreen
    
  else
    
    resizeWindow winWidth, winHeight
    
  end if
  
  'Need to set this flag so that the paint event knows it can do stuff since the displayEngine has been
  'set up.
  
  initialized = true
End Sub

c_displayEngine.resizeWindow:
Sub resizeWindow(width as integer, height as integer)
  'This method will resize the bounding window to the width and height passed.
  'After the resize, it automatically calls the enableScrollBars method to update
  'the scrollbars as needed.
  
  
  //me.window.width = width
  //me.window.height = height
  
  enableScrollBars
End Sub

c_displayEngine.zoomToScreen:
Sub zoomToScreen()
  dim sh as integer
  dim sw as integer
  dim winWidth as integer
  dim winHeight as integer
  dim zoomX as integer
  dim zoomY as integer
  
  
  'This method will determine the zoom value needed to resize the picture to fill the screen as much
  'as possible. After the zoom has been determined, the window is resized accordingly.
  
  
  'First, figure out how large the screen is. The subtracted values are arbitrary values
  'to account for the menu bar and window borders until I can come up with something
  'a bit more portable.
  
  sh = screen( 0 ).height - 56
  sw = screen( 0 ).width - 16
  
  
  'Now that we now the screen dimensions, we need to figure out what zoom value would fill
  'each dimension (not necessarily proportionally at this point).
  
  zoomX = ( sw / ( picWidth + dx_width ) ) * 100
  zoomY = ( sh / ( picHeight + dy_height ) ) * 100
  
  
  'Now that we have two different zoom values, we take the smaller of the two and resize the window
  'to redisplay the picture at that zoom value.
  
  if zoomX > zoomY then
    
    zoom = zoomY
    
  else
    
    zoom = zoomX
    
  end if
  
  getDisplaySizes
  
  resizeWindow ( picWidth + dx_width ) * ( zoom / 100 ), ( picHeight + dy_height ) * ( zoom / 100 )
End Sub

c_displayEngine.getDisplayPosition:
Protected Sub getDisplayPosition()
  
  'This method is used to determine where the picture should be placed in the displayEngine.
  'If the size of the Engine is greater than the displayed size of the picture,
  'the picture will be displayed centered in the Engine. If the displayed size of the picture
  'is greater than the size of the Engine, then 0 corrodinates are used.
  
  'This is a private method as it is only used to change private properities of the displayEngine.
  
  
  dim x as integer
  dim y as integer
  
  
  'Determine if the displayed width of the picture is smaller than the width of the engine.
  
  if disp_width < me.width then
    
    'Displayed width of the picture is smaller than the width of the engine. So we want to
    'calculate the proper x coordinate so the picture is displayed centered horizontally.
    
    x = ( me.width - disp_width ) \ 2
    
  else
    
    'It's not, so return 0.
    
    x = 0
    
  end if
  
  if disp_height < me.height then
    
    'Displayed height of the picture is smaller than the height of the engine. So we want to
    'calculate the proper y coordinate so the picture is displayed centered vertically.
    
    y = ( me.height - disp_height ) \ 2
    
  else
    
    'It's not, so return 0.
    
    y = 0
    
  end if
  
  
  'Now that we've found the coordinates, store them in the appropriate properties.
  
  disp_top = y
  disp_left = x
End Sub

c_displayEngine.getDisplaySizes:
Protected Sub getDisplaySizes()
  'This is a simple method used to determine the displayed size of the picture
  'based on the current zoom value.
  
  'This is a private method as it is only useful to the engine for determining 
  
  disp_width = picWidth * ( zoom / 100 )
  disp_height = picHeight * ( zoom / 100 )
End Sub

c_displayEngine.enableScrollBars:
Sub enableScrollBars()
  'This method is used to determine if the scroll bars should be enabled or not
  'and, if they are, will set the scroll bars' properties appropriately for the zoom value
  'and displayEngine size.
  
  
  'Determine if the scrollbars need to be enabled. If the displayed size of the picture
  'is greater than the size of the displayEngine, then one or both of the scrollbars
  'needs to be enabled.
  
  hScrollBar.enabled = ( disp_width > me.width + 1 )
  vScrollBar.enabled = ( disp_height > me.height + 1 )
  
  
  
  
  if hScrollBar.enabled then
    
    'The horizontal scrollbar is enabled, so we need to determine the appropriate
    'maximum, linestep and pagestep values for the zoom value and displayEngine size.
    
    hScrollBar.maximum = (( disp_width / ( zoom / 100 ) ) - ( me.width / ( zoom / 100 ) ))*2
    //hScrollBar.lineStep = ( disp_Width \ me.width ) *  ( zoom / 4 )
    
  else
    
    'It's not enabled, so we need to be sure that it's value property returns 0.
    
    hScrollBar.value = 0
    
  end if
  
  
  
  
  if vScrollBar.enabled then
    
    'The vertical scrollbar is enabled, so we need to determine the appropriate
    'maximum, linestep and pagestep values for the zoom value and displayEngine size.
    
    vScrollBar.maximum = (( disp_height / ( zoom / 100 ) ) - ( me.height / ( zoom / 100 ) ))*2
    //vScrollBar.lineStep = ( disp_height \ me.height ) * ( zoom / 4 )
    
  else
    
    'It's not enabled, so we need to be sure that it's value property returns 0.
    
    vScrollBar.value = 0
    
  end if
  
  
  
End Sub

c_displayEngine.updatePicturePosition:
Sub updatePicturePosition()
  'This method simply gets the displayEngine's scroll properties to the
  'current scrollbar values and then calls drawPicture to update the picture's position.
  
  
  scrollXPosition = hScrollBar.value
  scrollYPosition = vScrollBar.value
  
  drawPicture
End Sub

c_displayEngine.findZoomMenu:
Protected Sub findZoomMenu()
  dim ct as integer
  dim i as integer
  dim zm as control
  
  
  'The displayEngine also needs to figure out where it's zoomMenu is in the window
  '(if the user has set the "hasZoomMenu" property to true) and set it to
  'the appropriate size and location.
  
  'This is a private method because it needs only be run when the displayEngine is first created.
  
  
  
  if hasZoomMenu then
    
    'Yes, the displayEngine will be using a zoomMenu.
    
    
    'First, figure out how many controls are in the window.
    
    ct = me.window.controlCount
    
    
    'Step through each of the controls and see if any of them are what we are looking for.
    'Specifically: a bevelButton with its super set to "c_de_zoomMenu"
    
    for i = 0 to ct
      
      'Get a temporary reference to the control at the current index.
      
      zm = me.window.control( i )
      
      
      'Check to see if it is a "c_de_zoomMenu"
      
      if zm isa c_de_zoomMenu then
        
        'It is, so let's use this bevelButton for that purpose.
        'We need to set up a cross reference so that each control can see each other when needed.
        
        zoomMenu = c_de_zoomMenu( zm )
        zoomMenu.parent = self
        
      end if
      
    next
    
    'We've found the zoom menu, time to set it's size and position properties. Also set the
    'appropriate lock properties so the zoom menu stays where it should in relation to the
    'display engine if the window is resized.
    
    'Again, note the lack of error checking in this development version of the displayEngine.
    
    zoomMenu.height = 16
    zoomMenu.width = 80
    zoomMenu.left = me.left
    zoomMenu.top = me.top + me.height
    
    zoomMenu.lockBottom = me.lockEdges
    zoommenu.lockLeft = me.lockEdges
    
  end if
End Sub

c_displayEngine.setZoom:
Sub setZoom(z as integer)
  'This method simply sets the current zoom value to the passed integer and calls drawPicture.
  'After the redraw, it also updates the zoomMenu's zoom value display, if the displayEngine
  'has a zoom menu attached to it.
  
  
  zoom = z
  
  drawPicture
  
  if hasZoomMenu then
    zoomMenu.setZoomDisplay zoom
  end if
  
  enableScrollBars()
  winview.sliderZoom.value=zoom
End Sub

c_displayEngine.zoomToWindow:
Sub zoomToWindow()
  dim winWidth as integer
  dim winHeight as integer
  dim zoomX as integer
  dim zoomY as integer
  
  'This method is used to determine a zoom value that will fit the picture in the
  'space available at the current displayEngine size. The window is not resized in this
  'method, rather the picture is scaled to fit in available space.
  
  
  'We need to figure out what zoom value would fill
  'each dimension (not necessarily proportionally at this point).
  
  zoomX = ( me.width / picWidth ) * 100
  zoomY = ( me.height / picHeight ) * 100
  
  
  'Now that we have two different zoom values, we take the smaller of the two and set the zoom value
  'to redisplay the picture at that zoom value.
  
  if zoomX > zoomY then
    
    zoom = zoomY
    
  else
    
    zoom = zoomX
    
  end if
  
  
  'Once we have the final zoom value, call setZoom to resize the picture appropriately.
  
  getDisplaySizes
  setZoom
End Sub

c_displayEngine.setZoom:
Protected Sub setZoom()
  'This setZoom method is private so it can only be used by the displayEngine to set the displayed
  'zoom to the current zoom value (which it does by simply calling the public setZoom method.
  
  setZoom( zoom )
End Sub

c_displayEngine.findFrameRect:
Protected Sub findFrameRect()
  dim ct as integer
  dim i as integer
  dim fr as control
  
  
  'This method provides a way for the cDisplayEngine to find the c_de_frameRect control which
  'provides a "framed" appearance when the control is not against the edges of the window.
  
  'This is a private method because it needs only be run when the displayEngine is first created.
  
  
  
  'First, figure out how many controls are in the window.
  
  ct = me.window.controlCount
  
  
  'Step through each of the controls and see if any of them are what we are looking for.
  'Specifically: a canvas with its super set to "c_de_frameRect"
  
  for i = 0 to ct
    
    'Get a temporary reference to the control at the current index.
    
    fr = me.window.control( i )
    
    
    'Check to see if it is a "c_de_frameRect"
    
    if fr isa c_de_frameRect then
      
      'It is, so let's use this canvas for that purpose. Make sure the control knows it has a frameRect.
      
      hasFrameRect = true
      frame = c_de_frameRect( fr )
      
      'Since we will have a frame, we need to set it to the appropriate dimensions.
      
      frame.left = me.left - 1
      frame.top = me.top - 1
      frame.width = me.width + 17
      frame.height = me.height + 17
      
      frame.lockLeft = me.lockEdges
      frame.lockTop = me.lockEdges
      frame.lockRight = me.lockEdges
      frame.lockBottom = me.lockEdges
      
    else
      
      hasFrameRect = false
      
    end if
    
  next
End Sub

c_displayEngine.MouseUp:
Sub MouseUp(X As Integer, Y As Integer)
  dim rewidth, reheight as integer
  dim fatorW,fatorH as double
  dim ppp as picture
  
  rewidth=xend-xinit
  reheight=yend-yinit
  
  if rewidth<0 then
    rewidth=rewidth*(-1)
    xinit=x
  end if
  if reheight<0 then
    reheight=reheight*(-1)
    yinit=y
  end if
  
  me.graphics.DrawRect xinit,yinit,rewidth,reheight
  
  'fatorw=me.width/rewidth
  'fatorh=me.height/reheight
  
  fatorw=p.width/rewidth
  fatorh=p.height/reheight
  
  if fatorw<fatorh then
    reheight=me.height/fatorw
  else
    rewidth=me.width/fatorh
  end if
  '
  'ppp=newpicture(me.width,me.height,screen(0).depth)
  'ppp.graphics.drawpicture p,0,0,me.width,me.height,xinit,yinit,rewidth,reheight
  'me.graphics.drawpicture ppp,0,0
  'p=ppp
  
  zoom=zoom+fatorw*100
  getDisplaysizes
  //enablescrollBars
  //scrollxPosition=scrollxPosition
  //updatePicturePosition
  setZoom(zoom)
  drawPicture
End Sub

c_displayEngine.MouseDrag:
Sub MouseDrag(X As Integer, Y As Integer)
  me.graphics.clearRect 0,0,me.width,me.height
  me.graphics.drawpicture p,0,0
  me.graphics.DrawRect xinit,yinit,x-xinit,y-yinit
  xend=x
  yend=y
End Sub

c_displayEngine.MouseDown:
Function MouseDown(X As Integer, Y As Integer) As Boolean
  xinit=x
  yinit=y
  return true
End Function

c_displayEngine.Paint:
Sub Paint(g As Graphics)
  'To prevent nilObjectException errors, only allow the paint event to do anything
  'AFTER the display canvas has been initialized by calling the initialize method.
  
  if initialized then
    drawPicture
  end if
End Sub

c_displayEngine.Open:
Sub Open()
  
  findScrollBars
  
  backgroundColor = rgb( 0, 0, 0 )
  zoom = 50
  
  Open
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

Module.radiosfalse:
Sub radiosfalse()
  winpalheta.radioAlterar.value=false
  winpalheta.radioAprovado.value=false
  winpalheta.radioAprovadoComAlteracao.value=false
End Sub

c_de_frameRect.Paint:
Sub Paint(g As Graphics)
  'c_de_frameRect added 09/25/00
  'by Steve LoBasso
  'provides a "framed" effect to the c_displayEngine when it is not against the corners of the window.
  
  g.DrawRect(0, 0, me.width - 1, me.height - 1)
End Sub

c_de_zoomMenu.setZoomDisplay:
Sub setZoomDisplay(zoom as integer)
  me.caption = str( zoom ) + "%"
End Sub

c_de_zoomMenu.Action:
Sub Action()
  select case me.menuValue
    
  case 0
    
    parent.setZoom( 400 )
    
  case 1
    
    parent.setZoom( 200 )
    
  case 2
    
    parent.setZoom( 100 )
    
  case 3
    
    parent.setZoom( 75 )
    
  case 4
    
    parent.setZoom( 50 )
    
  case 5
    
    parent.setZoom( 25 )
    
  case 8
    
    otherzoom
    
  case 7
    
    parent.zoomToWindow
    
  end select
End Sub

c_de_zoomMenu.Open:
Sub Open()
  me.hasMenu = 1
  
  me.addRow "400%"
  me.addRow "200%"
  me.addRow "100%"
  me.addRow "75%"
  me.addRow "50%"
  me.addRow "25%"
  me.addSeparator
  me.addRow "Ajustar a p‡gina"    // <-- CONVERTED
  me.addRow "Outros..."
  
  me.caption = "50%"
  me.menuValue = 4
End Sub

c_de_scrollBar.ValueChanged:
Sub ValueChanged()
  parent.updatePicturePosition
End Sub

winaguarde.Timer1.Action:
Sub Action()
  select case alertatext.text
  case "Aguarde..."
    alertatext.text="Aguarde"
  case "Aguarde"
    alertatext.text="Aguarde."
  case "Aguarde."
    alertatext.text="Aguarde.."
  case "Aguarde.."
    alertatext.text="Aguarde..."
  end select
End Sub

