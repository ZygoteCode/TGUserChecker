using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class Program
{

    public static void Main()
    {
        Console.Title = "TGUserChecker | Made by https://github.com/ZygoteCode/";

        if (!File.Exists("users.txt"))
        {
            Logger.LogError("The file 'users.txt' does not exist. Please, create and fill it with all the usernames to check.");
            goto exit;
        }

        Logger.LogSuccess("Succesfully found the file 'users.txt', parsing.");
        string[] users = File.ReadAllLines("users.txt");

        if (users.Length == 0)
        {
            Logger.LogError("No usernames are found in the file 'users.txt'.");
            goto exit;
        }

        List<string> validUsernames = new List<string>();

        foreach (string user in users)
        {
            if (Utils.IsTelegramUsernameValid(user))
            {
                validUsernames.Add(user);
            }
        }

        if (validUsernames.Count == 0)
        {
            Logger.LogError($"Loaded {users.Length} usernames from the file 'users.txt' but no one of the loaded usernames has a valid format.");
            goto exit;
        }

        Logger.LogSuccess("Succesfully parsed all usernames from 'users.txt'.");
        Logger.LogInfo($"Usernames loaded: {users.Length}, real valid usernames to check: {validUsernames.Count}");

        string rootDir = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1) + ":";
        Logger.LogInfo($"Retrieved the main root dir: {rootDir}\\");

        UndetectedChromeDriver.UndetectedChromeDriver driver =
            UndetectedChromeDriver.UndetectedChromeDriver.Create
        (
            driverExecutablePath: Path.GetFullPath("chromedriver.exe"),
            browserExecutablePath: rootDir + "\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
        );

        Logger.LogSuccess("Succesfully started the Chrome Driver instance.");
        Logger.LogInfo($"Driver executable path: {Path.GetFullPath("chromedriver.exe")}");
        Logger.LogInfo($"Browser executable path: {rootDir + "\\Program Files\\Google\\Chrome\\Application\\chrome.exe"}");

        driver.MaximizeWindow();
        Logger.LogInfo("Maximized the window of Google Chrome.");
        string _validUsernames = "", _invalidUsernames = "";
        int _validUsernamesCount = 0, _invalidUsernamesCount = 0;

        foreach (string username in validUsernames)
        {
            Logger.LogInfo($"Checking username '{username}'.");
            bool valid = false;
            driver.GoToUrl($"https://t.me/{username}/");

            while (!driver.IsPageReady())
            {
                Thread.Sleep(10);
            }

            Thread.Sleep(1000);

            try
            {
                if (driver.FindElement(OpenQA.Selenium.By.ClassName("tgme_icon_user")) == null)
                {
                    valid = true;
                }
            }
            catch
            {
                valid = true;
            }

            if (valid)
            {
                Logger.LogSuccess($"The username '{username}' exists and is valid.");
                _validUsernamesCount++;

                if (_validUsernames == "")
                {
                    _validUsernames = username;
                }
                else
                {
                    _validUsernames += "\r\n" + username;
                }
            }
            else
            {
                Logger.LogError($"The username '{username}' does not exist and is not valid.");
                _invalidUsernamesCount++;

                if (_invalidUsernames == "")
                {
                    _invalidUsernames = username;
                }
                else
                {
                    _invalidUsernames += "\r\n" + username;
                }
            }
        }

        driver.Quit();

        File.WriteAllText("valid.txt", _validUsernames);
        File.WriteAllText("invalid.txt", _invalidUsernames);

        Logger.LogSuccess($"Succesfully finished checking all the {validUsernames.Count} usernames.");
        Logger.LogSuccess($"Valid & existing usernames: {_validUsernamesCount}. Saved to 'valid.txt' file.");
        Logger.LogSuccess($"Invalid & non-existing usernames: {_invalidUsernamesCount}. Saved to 'invalid.txt' file.");

        exit: Logger.LogWarning("Press ENTER to exit from the program.");
        Console.ReadLine();
        return;
    }


}
