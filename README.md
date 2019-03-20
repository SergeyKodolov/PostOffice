# PostOffice

В файле data.txt необходимо указать:  
  * E-Mail Яндекс.Почты  
  * Пароль  
  * Путь до файла БД  
  * API key для DaData.ru  
  
Установка службы:  
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe ***ПутьДоФайла***\PostOfficeService.exe  

и удаление:  
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe -u ***ПутьДоФайла***\PostOfficeService.exe  

В файле ***PostOfficeDataBase/App.config*** необходимо изменить путь к БД
