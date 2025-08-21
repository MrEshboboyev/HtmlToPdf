namespace HtmlToPdf.Contracts;

public enum ActivityType
{
    SipTelephony,    // Приложения для SIP-телефонии (green)
    Accounting,      // Бухгалтерия (green) 
    SystemInterface, // Интерфейс системы (gray)
    Other,           // Всё остальное (gray)
    MicrosoftExe,    // microsip.exe (green)
    OneСExe,         // 1cv8c.exe (green)
    ExplorerExe,     // explorer.exe (gray)
    BrowserExe,      // browser.exe (gray)
    MsiExec,         // msiexec.exe (gray)
    YandexRu         // yandex.ru (gray)
}
