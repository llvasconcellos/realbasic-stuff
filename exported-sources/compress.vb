Window1.PushButton1.Action:
Sub Action()
  dim original,compressed as folderItem
  dim readfromfile,writetofile as binaryStream
  #if targetcarbon then
    dim readresource as resstream
  #endif
  dim parametros,data as string
  dim i as integer
  dim valor,filetotal,position,controle as double
  original=getopenFolderItem("allfiles")
  if original<>nil then
    filetotal=original.length+original.resourceForkLength
    readfromfile=original.openasbinaryFile(false)
    #if targetcarbon then
      readresource=original.openasResStream(false)
    #endif
    compressed=getfolderitem(original.name+".oct")
    writetofile=compressed.createbinaryFile("allfiles")
    #if targetcarbon then
      parametros=original.name+chr(9)+"maccreator="+original.maccreator+chr(9)+"mactype="+original.mactype+chr(9)+"mac"
    #endif
    #if targetwin32 then
      parametros=original.name+chr(9)+"pc"
    #endif
    i=lenb(parametros)
    writetofile.write format(i,"0000")
    writetofile.write parametros
    #if targetcarbon then
      writetofile.write format(original.getFileFlags,"00000000")
      data=readresource.read(readresource.Length)
      data=compress(data,9)
      writetofile.write format(lenb(data),"00000000")
      writetofile.write data
      readresource.close
    #endif
    
    while not readfromfile.eof
      data=compress(readfromfile.read(1048576),9)
      controle=lenB(data)
      writetofile.write format(controle,"00000000")
      writetofile.write data
    wend
    readfromfile.close
    writetofile.close
  end if
End Sub

Window1.PushButton2.Action:
Sub Action()
  dim compactado,descompactado as folderItem
  dim readfromfile,writetofile as binaryStream
  dim readresource as resstream
  dim res as resourceFork
  dim name as string
  dim i,fileflags as integer
  dim parametros,macc,mact,data,os as string
  
  compactado=getopenFolderItem("allfiles")
  if compactado<>nil then
    readfromfile=compactado.openasbinaryFile(false)
    
    data=readfromfile.read(4)
    i=val(data)
    
    parametros=readfromfile.read(i)
    
    name=nthField(parametros,chr(9),1)
    
    os=nthField(parametros,chr(9),countfields(parametros,chr(9)))
    
    macc=nthField(parametros,chr(9),2)
    macc=nthField(macc,"=",2)
    
    mact=nthField(parametros,chr(9),3)
    mact=nthField(mact,"=",2)
    
    descompactado=getfolderItem(name)
    writetofile=descompactado.createbinaryFile("allfiles")
    if os="mac" then
      res=descompactado.createResourceFork("allfiles")
      res.close
      data=readfromfile.read(8)
      fileflags=val(data)
      data=readfromfile.read(8)
      i=val(data)
      data=readfromfile.read(i)
      data=uncompress(data,1048576)
      readresource=descompactado.openasResStream(true)
      readresource.write data
      readresource.close
    end if
    while not readfromfile.eof
      data=readfromfile.read(8)
      i=val(data)
      data=readfromfile.read(i)
      data=uncompress(data,1048576)
      writetofile.write data
    wend
    readfromfile.close
    writetofile.close
    if os="mac" then
      i=descompactado.setFileFlags(fileflags)
      descompactado.maccreator=macc
      descompactado.mactype=mact
    end if
  end if
End Sub

