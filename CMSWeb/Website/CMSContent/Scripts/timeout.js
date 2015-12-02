var timeoutWarningID;
var timeoutID;
var statusBarTimerID;
var statusBarTimerID2;

//**** For Testing only ********
//var defaultTimeoutMilliseconds = 60000;  // 58 minutes
//var timeWaitForFinalPopUp = 120000; //60 minutes
//var howLong = 60000; //2 minutes

//******Prod only settings *******
var defaultTimeoutMilliseconds = 3300000;  // 55 minutes - 5 minutes before the session timout
var timeWaitForFinalPopUp = 3600000; //60 minutes - ASP session timeout 
var howLong = 300000; //5 minutes


// ----------------------------------------------------------------------------
// this sets up the timeout counter for warning message and also for timeout message
// ----------------------------------------------------------------------------

function setupTimeout()
{
    window.clearTimeout(timeoutWarningID);
    window.clearTimeout(timeoutID);
    window.clearTimeout(statusBarTimerID);
    window.clearTimeout(statusBarTimerID2);

    warningTimeoutMilliseconds = getWarningTimeoutInMilliseconds();

    timeoutID = window.setTimeout('displayTimeoutMsg()', timeWaitForFinalPopUp);
    timeoutWarningID = window.setTimeout('displayTimeoutWarningMsg()', warningTimeoutMilliseconds);
    statusBarTimerID = window.setTimeout('displayStatusBarMessage()', 500);
    statusBarTimerID2 = window.setTimeout('removeStatusBarMessage()', 10000);
}

function displayStatusBarMessage(){
   window.status="You have "+timeWaitForFinalPopUp/60000+" minutes left in your current session";
}

function removeStatusBarMessage(){
   window.status="";
}



// ----------------------------------------------------------------------------
// this shows the warning message in a new window
// ----------------------------------------------------------------------------
function displayTimeoutWarningMsg()
{
    timeoutWarningMsg = getTimeoutWarningMsg();

    var height=240;
    var width=330;
    xTop = screen.width/2 - (width/2);
    yTop = screen.height/2 - (height/2);
	    
	var toPopUpWindowOptions    = "toolbar=0" + ",location=0" + ",directories=0"
								 + ",status=0" + ",menubar=0" + ",scrollbars=0"
								 + ",resizable=0"  + ",width=330" + ",height=240" + ",left=" + xTop + ",top=" + yTop;

 
    timeoutWin = window.open("", "NewWindow", toPopUpWindowOptions, true);
    timeoutDoc = timeoutWin.document;

    timeoutDoc.writeln('<html>\n<head>\n<title>Application Closing<\/title>\n');

    timeoutDoc.writeln( '<scr' + 'ipt language="JavaScript" type="text/JavaScript">function hover(ref, classRef) { eval(ref).className = classRef; }<\/scri' + 'pt>');

    timeoutDoc.writeln('<\/head>\n');

    s  = '';
    s += '<body bgcolor="#FFFFFF" link="#0000CC" vlink="#FF0000" alink="#CECECE" ';
    s += 'onLoad="window.setTimeout(';
    s += "'this.close()', ";
    s += howLong;
    s += ');';
    s += '">\n';

    timeoutDoc.write(s);

    s = '';
    s += '<table border=0 cellpadding=5 cellspacing=0 align=center ';
    s += 'bgcolor="#FFFFFF" width=300>\n';
    s += '<tr><td bgcolor="#FFFFFF" width=100% align=center>\n';
    s += '\n';
    s += '<\/td><\/tr>\n';
    s += '<tr><td>\n';

    timeoutDoc.write(s);

    s = '';
    s += '<br>';
    s += '<font face="arial" size="2">\n';
    s += timeoutWarningMsg;
    s += '\n<\/font>\n';
    s += '<\/td><\/tr>\n';
    s += '<tr><td align=center>\n';
    s += '<br>\n';

    timeoutDoc.write(s);

	s = '';
	s += '<br>';
	s += '<A href=\n';
	s += '"javascript:window.opener.setupTimeout();window.opener.openJsp();window.opener.focus();this.close();">';
	s += '<img border="0" alt="Press this button to continue your session" src="/images/ok.gif" align="center"></a>';

	s += '&nbsp;&nbsp;<A href=\n';
	s += '"javascript:window.opener.openLogoutPage();this.close();">';
	s += '<img border="0" alt="Press this button to exit your session" src="/images/Exit.gif" align="center"></a>';

    timeoutDoc.write(s);

    timeoutDoc.writeln('<\/td><\/tr><\/table><\/body>\n<\/html>\n');
    timeoutDoc.close();
    timeoutWin.focus();
}


// ----------------------------------------------------------------------------
// this shows the time out message(alert)
// ----------------------------------------------------------------------------
function displayTimeoutMsg()
{
    timeoutMsg = getTimeoutMsg();
    alert(timeoutMsg);
    openLogoutPage();
}

// ----------------------------------------------------------------------------
// returns the time in milli seconds after which warning message needs to be displayed
// ----------------------------------------------------------------------------
function getWarningTimeoutInMilliseconds()
{
    return defaultTimeoutMilliseconds;
}


// ----------------------------------------------------------------------------
// returns the warning message text
// ----------------------------------------------------------------------------
function getTimeoutWarningMsg()
{
timeoutWarningMsg = "Your online session with eWealthManager is about to be timed out. As a security precaution, sessions end after "+timeWaitForFinalPopUp/60000+" minutes of inactivity. <br><br>Click <b>Leave Open</b> button below to continue your current session Or click <b>Close Session</b> button to end your session now.";

return timeoutWarningMsg;
}


// ----------------------------------------------------------------------------
// returns the timeout message text
// ----------------------------------------------------------------------------
function getTimeoutMsg()
{
timeoutMsg = "Your online session with eWealthManager has been timed out. As a security precaution, sessions are ended after "+timeWaitForFinalPopUp/60000+" minutes of inactivity. You can sign in again to resume.";

    return timeoutMsg;
}

function openJsp()
{
	my_window =window.open('/DNeWM/opendoc.aspx?page_name="Dummy.aspx"','openDocuments','toolbar=yes,resizable=yes,menubar=yes');
	my_window.document.write("Session time");
	my_window.close();

}

function openLogoutPage()
{
 self.location="/DNeWM/LogOff.aspx"
}