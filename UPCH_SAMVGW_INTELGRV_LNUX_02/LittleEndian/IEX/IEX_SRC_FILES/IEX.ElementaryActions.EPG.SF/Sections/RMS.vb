Imports FailuresHandler
Imports OpenQA.Selenium
Imports OpenQA.Selenium.Firefox
Imports OpenQA.Selenium.Support.UI

Public Class RMS
    Inherits IEX.ElementaryActions.EPG.RMS
    Dim failureCount As Integer = 0
    Dim _UI As UI
    Private Shadows _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.UI)
        MyBase.New(_pIex, UI)
        Me._iex = _pIex
        _UI = UI
        _Utils = _UI.Utils
    End Sub

    Public BoxId As String
    Public rmsURL As String
    Public usrname As String
    Public passwd As String
    Public LoginTextId As String
    Public PasswdTextId As String
    Public SignInBtnId As String
    Public CPEIdTBoxId As String
    Public SearchBtnId As String
    Public DeviceRBtnId As String
    Public Value As String = Nothing
    Public updated As String
    Public path1 As String
    Public path2 As String
    Public path3 As String
    Public path4 As String
    Public epgLanguagePath As String
    Public epgTimeOutPath As String
    Public SubtitleDisplayPath As String
    Public SubtitleLanguagePath As String
    Public swithToStandbyWhenInactivePath As String
    Public TabId As String
  
    ''' <summary>
    ''' Login To Panorama page 
    ''' </summary>
    ''' <param name="driver"></param>
    ''' <remarks></remarks>

    Public Overrides Sub RmsLoginValidation(ByVal driver As FirefoxDriver)

        Try
            Try
                rmsURL = _Utils.GetValueFromTestIni("TEST PARAMS", "RMS_URL")
                passwd = _Utils.GetValueFromTestIni("TEST PARAMS", "PASSWD")
            Catch ex As Exception
                _Utils.LogCommentWarning("RMS_URL is not defined in the Test ini, taking RMS_URL from Project ini")
                rmsURL = ""
                passwd = ""
            End Try

            If rmsURL = "" Then
                rmsURL = _Utils.GetValueFromProject("RMS", "RMS_URL")
            End If

            usrname = _Utils.GetValueFromProject("RMS", "USER_ID")

            If passwd = "" Then
                passwd = _Utils.GetValueFromProject("RMS", "PASSWD")
            End If
            LoginTextId = _Utils.GetValueFromBrowser("BROWSER_ID", "LoginTBox_Id")
            PasswdTextId = _Utils.GetValueFromBrowser("BROWSER_ID", "PasswdTBox_Id")
            SignInBtnId = _Utils.GetValueFromBrowser("BROWSER_ID", "SignInBtn_Id")
            driver.Manage().Timeouts.ImplicitlyWait(TimeSpan.FromSeconds(120))
            driver.Navigate().GoToUrl(rmsURL)
            driver.Manage().Window.Maximize()

            ''Login username and password
            driver.FindElement(By.Id("" + LoginTextId + "")).Click()
            driver.FindElement(By.Id("" + LoginTextId + "")).Clear()
            driver.FindElement(By.Id("" + LoginTextId + "")).SendKeys(usrname)
            driver.FindElement(By.Id("" + PasswdTextId + "")).SendKeys(passwd)
            driver.FindElement(By.Id("" + SignInBtnId + "")).Click()


        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.LoginFailure, "RMS Login Failure with url" + ex.Message))
        Finally
            _Utils.StartHideFailures("Rms Login validation")

        End Try
    End Sub

    ''' <summary>
    ''' Enters CpeId
    ''' </summary>
    ''' <param name="driver"></param>
    ''' <param name="cpeId"></param>
    ''' <remarks></remarks>
    Public Overrides Sub EnterCpeId(ByVal driver As FirefoxDriver, ByVal cpeId As String)
        Try
            ''Get Browser Ids From Browser ini File
            BoxId = cpeId
            CPEIdTBoxId = _Utils.GetValueFromBrowser("BROWSER_ID", "CPEIdTBox_Id")
            DeviceRBtnId = _Utils.GetValueFromBrowser("BROWSER_ID", "DeviceRBtn_Id")
            SearchBtnId = _Utils.GetValueFromBrowser("BROWSER_ID", "SearchBtn_Id")

            ''Select radio button Device and Enter the CPE ID
            ''driver.FindElement(By.Id("" + DeviceRBtnId + "")).Click()
            driver.FindElement(By.Id("" + CPEIdTBoxId + "")).Click()
            driver.FindElement(By.Id("" + CPEIdTBoxId + "")).SendKeys(cpeId)


            '' Search the Box Based on CPE ID
            driver.FindElement(By.ClassName("" + SearchBtnId + "")).Click()


        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.BoxIdFormatFailure, "BoxId is Incorrect Or not Entered " + ex.Message))
        Finally
            _iex.ForceHideFailure()
            If cpeId <> "" Then
                _Utils.LogCommentInfo(cpeId)
            End If

        End Try
    End Sub

    ''' <summary>
    ''' Performs Selection of Browser Tabs
    ''' </summary>
    ''' <param name="driver"></param>
    ''' <param name="BrowserTabControlId"></param>
    ''' <remarks></remarks>
    Public Overrides Sub SelectTab(ByVal driver As FirefoxDriver, ByVal BrowserTabControlId As String, Optional ByVal FindElementby As EnumFindElementBy = EnumFindElementBy.Xpath)

        TabId = BrowserTabControlId
        Try
            If EnumFindElementBy.Xpath Then


                If (BrowserTabControlId.Contains("Settings")) Then

                    If _Utils.GetValueFromEnvironment("Project").ToUpper() = "IPC" Then


                        BrowserTabControlId = BrowserTabControlId + " [3]"

                    Else

                        BrowserTabControlId = BrowserTabControlId + " [2]"
                    End If

                ElseIf (BrowserTabControlId.Contains("Parameters")) Then

                    If _Utils.GetValueFromEnvironment("Project").ToUpper() = "IPC" Then


                        BrowserTabControlId = BrowserTabControlId + " [2]"

                    Else

                        BrowserTabControlId = BrowserTabControlId + " [1]"
                    End If


                End If
                driver.FindElement(By.XPath("" + BrowserTabControlId + "")).Click()
            Else
                driver.FindElement(By.CssSelector("" + BrowserTabControlId + "")).Click()
            End If

        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigateFailure, "Unable to Navigate Tab" + ex.Message))
            _iex.FailStep("Unable to Select the Tab control")
        Finally
            _iex.ForceHideFailure()
            If BrowserTabControlId <> "" Then
                _Utils.LogCommentInfo(BrowserTabControlId)
            End If
            _iex.LogComment("Successfully Selected the" + BrowserTabControlId + "tab")
        End Try
    End Sub
  
    ''' <summary>
    ''' Performing qucick Action Functionalities
    ''' </summary>
    ''' <param name="driver"></param>
    ''' <param name="QuickActionControlId"></param>
    ''' <param name="QuickActionConfirmId"></param>
    ''' <remarks></remarks>
    Public Overrides Sub QuickActionSelect(ByVal driver As FirefoxDriver, ByVal quickActionControlId As String, ByVal quickActionConfirmId As String)

        Try
            driver.FindElement(By.XPath("//span[@title='" + quickActionControlId + "']")).Click()

            driver.FindElement(By.XPath("//span[text()= '" + quickActionConfirmId + "']")).Click()
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigateFailure, "Unable to Perform QuickAction" + ex.Message))
            _iex.FailStep("Failed to Perform Quick Action")
        Finally
            _iex.ForceHideFailure()
            If quickActionControlId <> "" Then
                _Utils.LogCommentInfo(quickActionControlId)
            End If
        End Try
    End Sub
    ''' <summary>
    ''' Fetch the Parameter Values From The panorama page
    ''' </summary>
    ''' <param name="driver"></param>
    ''' <param name="paramId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetParameterValues(ByVal driver As FirefoxDriver, ByVal paramId As String) As String
        Dim wait As New WebDriverWait(driver, TimeSpan.FromSeconds(15))
        Try

            _Utils.LogCommentInfo("Clicking on the input box to send keys")
            wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("v-filterselect-input")))
           _iex.Wait(2)
            _Utils.LogCommentInfo("Clicking on v-filterselect-input")
            driver.FindElement(By.ClassName("v-filterselect-input")).Click()

            wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("v-filterselect-input")))
            _iex.Wait(2)
            _Utils.LogCommentInfo("Sending the keys " + paramId)
            driver.FindElement(By.ClassName("v-filterselect-input")).SendKeys("" + paramId + "")
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = '" + paramId + "']")))
            _iex.Wait(2)
            _Utils.LogCommentInfo("Clicking on span text " + paramId)
            driver.FindElement(By.XPath("//span[text() = '" + paramId + "']")).Click()
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[text() = 'Retrieve']")))

            _iex.Wait(2)
            ''Perform Retrieve to get the latest values
            _Utils.LogCommentInfo("Perform Retrieve to get the latest values")
            driver.FindElement(By.XPath("//span[text() = 'Retrieve']")).Click()
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("(//input[@type='text'])[2]")))
            _Utils.LogCommentInfo("Refresh the webpage to ensure that value is updated")

            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("(//input[@type='text'])[2]")))
            driver.Navigate().Refresh()
            _iex.Wait(10)
            _Utils.LogCommentInfo("Getting text from //input[@type='text'])[2] ")
            value = driver.FindElement(By.XPath("(//input[@type='text'])[2]")).GetAttribute("value").ToString()
            failureCount = 0
        Catch ex As Exception
            _Utils.LogCommentWarning(ex.Message)
            failureCount = failureCount + 1
            _iex.ForceHideFailure()
            _Utils.LogCommentInfo("Send Keys Value may be incorrect and exception occurred")
            _Utils.LogCommentInfo("As Exception occured , reentering the box id and fetching the respective value ")
            EnterCpeId(driver, boxId)
            _Utils.LogCommentInfo("Succesfully Reentered the boxid ")
            _iex.Wait(5)
            SelectTab(driver, TabId)
            _Utils.LogCommentInfo("Succesfully selected the paramters tab second time ")
            _iex.Wait(3)
            If (failureCount < 3) Then
                GetParameterValues(driver, paramId)
                _Utils.LogCommentInfo("Succesfully reexecuted the getparameters Fuction ")

            Else
                _Utils.LogCommentInfo("Retried the execution of get paramater twice and even failed so exiting from reexecution")
                failureCount = 0
            End If

        Finally
            If Value <> "N/A" Then
                _Utils.LogCommentInfo("Retrived value from Panorama page for Get values: " + Value)
            End If
        End Try

        Return Value

    End Function
    ''' <summary>
    ''' Fetch the Parameter Values From The panorama page
    ''' </summary>
    ''' <param name="driver"></param>
    ''' <param name="setParameterPath"></param>
    ''' <remarks></remarks>
    Public Overrides Function SetParameterValues(ByVal driver As FirefoxDriver, ByVal setParameterPath As String, ByVal applyButtonPath As String, ByVal divTabName As String, ByVal sendValue As String) As Boolean
        Try

            ''select the parameter setting Tab
            _iex.Wait(2)
            _Utils.LogCommentInfo("Selecting the section to navigate" + divTabName)
            driver.FindElement(By.XPath("//div[text()='" + divTabName + "']")).Click()
            _iex.Wait(2)
            _Utils.LogCommentInfo("Selecting the parameter path " + setParameterPath)
            driver.FindElement(By.XPath(setParameterPath + "div")).Click()
            _iex.Wait(2)
            '' send the value to be set

            _Utils.LogCommentInfo("Clearing the value to the setpath " + setParameterPath)
            driver.FindElement(By.XPath("" + setParameterPath + "input")).Clear()
            _iex.Wait(2)
            _Utils.LogCommentInfo("Clicking on the input to send the keys " + setParameterPath)
            driver.FindElement(By.XPath("" + setParameterPath + "input")).Click()
            _iex.Wait(2)
            _Utils.LogCommentInfo("Inputting the  " + sendValue + "to the input box")
            driver.FindElement(By.XPath("" + setParameterPath + "input")).SendKeys(sendValue)
            _iex.Wait(2)
            _Utils.LogCommentInfo("Select the .gwt-MenuItem > span  after sending the required value.")
            ''click on the value on the dropdown list
            driver.FindElement(By.CssSelector("td.gwt-MenuItem > span")).Click()
            _iex.Wait(2)
            _Utils.LogCommentInfo("Selecting the Apply button to save")
            ''click on the apply button over panorama
            driver.FindElement(By.XPath(applyButtonPath)).Click()
            Return True
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigateFailure, "Unable to set Value" + ex.Message))
            Return False
        Finally
            _iex.ForceHideFailure()
        End Try

    End Function


End Class
