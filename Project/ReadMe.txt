команды для создания бд
Add-Migration "MessengerDB"
после в каждых местах где есть 2 User, в созданном командой файле заменять с ReferentialAction.Cascade на ReferentialAction.NoAction
Update-Database

