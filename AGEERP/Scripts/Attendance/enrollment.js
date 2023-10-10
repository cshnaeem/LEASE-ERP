var currentFormat = Fingerprint.SampleFormat.PngImage;
var count = 4;
//var state = document.getElementById('content-capture');

var FingerprintSdk = (function () {
    function FingerprintSdk() {
        var _instance = this;
        this.operationToRestart = null;
        this.acquisitionStarted = false;
        this.sdk = new Fingerprint.WebApi;
        this.sdk.onDeviceConnected = function (e) {
            // Detects if the deveice is connected for which acquisition started
            showMessage("Device connected");
        };
        this.sdk.onDeviceDisconnected = function (e) {
            // Detects if device gets disconnected - provides deviceUid of disconnected device
            showMessage("Device disconnected");
        };
        this.sdk.onCommunicationFailed = function (e) {
            // Detects if there is a failure in communicating with U.R.U web SDK
            showMessage("Communinication Failed")
        };
        this.sdk.onSamplesAcquired = function (s) {
            // Sample acquired event triggers this function

            CaptureEnrollment(s);
        };
        this.sdk.onQualityReported = function (e) {
            // Quality of sample aquired - Function triggered on every sample acquired
            document.getElementById("qualityInputBox").textContent = Fingerprint.QualityCode[(e.quality)];
        }
    }

    FingerprintSdk.prototype.startCapture = function () {
        document.getElementById("msg").textContent = "Please scan your fingure " + count + " times";
        if (this.acquisitionStarted) // Monitoring if already started capturing
            return;
        var _instance = this;
        showMessage("");
        this.operationToRestart = this.startCapture;

        this.sdk.startAcquisition(currentFormat, myVal).then(function () {
            _instance.acquisitionStarted = true;

            //Disabling start once started
         //   disableEnableStartStop();

        }, function (error) {
            showMessage(error.message);
        });
    };
    FingerprintSdk.prototype.stopCapture = function () {
        if (!this.acquisitionStarted) //Monitor if already stopped capturing
            return;
        var _instance = this;
        showMessage("");
        this.sdk.stopAcquisition().then(function () {
            _instance.acquisitionStarted = false;

            //Disabling stop once stoped
            disableEnableStartStop();

        }, function (error) {
            showMessage(error.message);
        });
    };
    FingerprintSdk.prototype.getInfo = function () {
        var _instance = this;
        return this.sdk.enumerateDevices();
    };

    FingerprintSdk.prototype.getDeviceInfoWithID = function (uid) {
        var _instance = this;
        return this.sdk.getDeviceInfo(uid);
    };
    return FingerprintSdk;
})();

window.onload = function () {
    test = new FingerprintSdk();
    onGetInfo();
};

function showMessage(message) {
    var _instance = this;
    //var statusWindow = document.getElementById("status");
    x = $("#status");
    if (x.length != 0) {
        x[0].innerHTML = message;
    }
}



function onStart() {
    test.startCapture();
}

function onStop() {
    test.stopCapture();
}

function onGetInfo() {
    var allReaders = test.getInfo();
    allReaders.then(function (sucessObj) {
        if (sucessObj.length > 0) {
            console.log(sucessObj[0]);
            this.myVal = sucessObj[0];
            onStart();
        }
        else {
            showMessage("Connect Reader Properly for Enrollment");
        }
    }, function (error) {
        showMessage(error.message);
    });
}
var template = []
var IsLoading = false;

function Reset() {
    count = 4;
    template = [];
    var image = document.getElementById("imagediv");
    image.src = "../Content/Attendance/scanok.png";
}