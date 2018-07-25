Window1.ListBox1.Open:
Sub Open()
  me.acceptfileDrop("allfiles")
End Sub

Window1.ListBox1.DropObject:
Sub DropObject(obj As DragItem)
  dim last as integer
  dim fd as folderItem
  dim pasta as string
  
  Do
    If Obj.FolderItemAvailable then
      fd=obj.folderItem
      me.addRow fd.name
      me.cell(me.lastIndex,1)=fd.absolutePath
    end if
  Loop until Not obj.NextItem
End Sub

Window1.PushButton2.Action:
Sub Action()
  dim t as OCTDecompress
  t=new OCTDecompress
  t.destinationpath=editfield1.text
  t.run
End Sub

OCTDecompress.headerlookup:
Sub headerlookup()
  dim type,path,name,maccreator,mactype,os,flagstring,data as string
  dim flags as integer
  dim i as integer
  dim fd as folderItem
  
  while not readfromfile.eof
    do
      i=val(readfromfile.read(4))
      header=readfromfile.read(i)
      
      type=nthField(header,chr(9),1)
      type=replace(type,"<header-:-","")
      type=nthField(type,"=",2)
      
      name=nthField(header,chr(9),2)
      name=nthField(name,"=",2)
      
      path=nthField(header,chr(9),3)
      path=nthField(path,"=",2)
      
      os=nthField(header,chr(9),countfields(header,chr(9)))
      os=nthField(os,"=",2)
      
      if os="mac>" then
        maccreator=nthField(header,chr(9),4)
        maccreator=nthField(maccreator,"=",2)
        
        mactype=nthField(header,chr(9),5)
        mactype=nthField(mactype,"=",2)
        
        flagstring=nthField(header,chr(9),6)
        flagstring=nthField(flagstring,"=",2)
        flags=val(flagstring)
      end if
      
      if type="folder" then
        if path="" then
          root=name+":"
          fd=getfolderItem(destinationpath+root)
          fd.createAsFolder
        else
          //for i=1 to countfields(path,":")-1
          //data=nthField(path,":",i)
          fd=getfolderItem(destinationpath+root+path)
          fd.createAsFolder
          //next
        end if
      end if
    loop until type="file"
    doit(type,name,os,path,maccreator,mactype,flags)
  wend
End Sub

OCTDecompress.doit:
Sub doit(type as string, name as string, os as string, path as string, maccreator as string, mactype as string, flags as integer)
  dim descompactado as folderItem
  dim writetofile as binaryStream
  dim data as string
  dim i as integer
  dim readresource as resstream
  dim res as resourceFork
  dim theend as boolean
  
  theend=false
  descompactado=getfolderItem(destinationpath+root+path)
  writetofile=descompactado.createbinaryFile("allfiles")
  
  if os="mac>" then
    res=descompactado.createResourceFork("allfiles")
    res.close
    data=readfromfile.read(8)
    i=val(data)
    data=readfromfile.read(i)
    data=uncompress(data,1048576)
    readresource=descompactado.openasResStream(true)
    readresource.write data
    readresource.close
  end if
  
  
  while not theend
    data=readfromfile.read(8)
    if data<>"*=NEXT=*" then
      i=val(data)
      data=readfromfile.read(i)
      data=uncompress(data,1048576)
      writetofile.write data
    else
      theend=true
    end if
  wend
  writetofile.close
  if os="mac>" then
    i=descompactado.setFileFlags(flags)
    descompactado.maccreator=maccreator
    descompactado.mactype=mactype
  end if
End Sub

OCTDecompress.Run:
Sub Run()
  dim i as integer
  
  for i=0 to window1.listbox1.ListCount-1
    compressed=getFolderItem(window1.listbox1.cell(i,1))
    readfromfile=compressed.openasbinaryFile(false)
    headerlookup
  next
End Sub

