# Conversion
Conversion is a program can convert string you input anywhere in Windows,and now it can help you input specal symbol,open other program and paste code fragment from source code file you set.
******
## How to use
First,you should write a table what define string you want to convert and the action you want to do when string has been converted.This table should be named as table.txt,and it should be placed in a directory at the same level as Conversion.exe.

The contents of the table should abide by the following rules:
* string you want to convert,action you want to do
* Comma (,) is used to separate them.And remember,don't input any unnecessary spcae.
* The key "|" or "\" (Oem5) on the keyboard is the switch to start conversion.
* The key space on the keyboard is the stop tag of conversion.
* String you want to convert is case - sensitive.
* "\run" and "\code" are tag of action.You should use space to separate tag of action and action you want to do.
* When you want to open other program,you can set the arguments of start info behind the path of program starting use "|" to separate. 

## Simple examples
* 1. Just input specal symbol
> aleph,ℵ
When you press Oem5 key and input "aleph" then press space bar,you will input "ℵ".
* 2. Open other program
> chrome,\run C:\Program Files (x86)\Google\Chrome\Application\chrome.exe
If you just want to open a program without arguments of start info.You can use the above format.
> partitionC,\run explorer.exe| c:
Remember,the space before arguments of start info "c:" is important.Is "| c:" but not "|c:".
* 3. Paste code fragment
> myCode,\code D:\1.ts
If the source code file is in a directory at the same level as Conversion.exe,you can also use under format.
> myCode,\code 1.ts
The space before the path of source code file is important,too.

## Final
Conversion has used keyboard hook of Win32API,so there is the possibility of being treated as a virus.But the source code is here...So...Emmmmmmmmmm...

And remeber,if you don't have special requirements, downloading the source code from here and compile these by yourself is the safest way.

JUST ENJOY IT!

Chao.Shengze && Fang.Jingxian

中文版本说明请参阅：
http://blog.csdn.net/chaoshengze/article/details/79407422