var currentFormat = Fingerprint.SampleFormat.PngImage;
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
        document.getElementById("msg").textContent = "Please scan your fingure ";
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
var IsLoading = false;
function CaptureEnrollment(s) {
    var empId = $('#EmployeeId').text();
    if (empId > 0) {
        if (currentFormat == Fingerprint.SampleFormat.PngImage) {
            var samples = JSON.parse(s.samples);
            localStorage.setItem("imageSrc", "data:image/png;base64," + Fingerprint.b64UrlTo64(samples[0]));
            var template = Fingerprint.b64UrlTo64(samples[0]);
            var image = document.getElementById("imagediv");
            image.src = localStorage.getItem("imageSrc");
            IsLoading = true;
            kendo.ui.progress($('.card'), true);
            $.ajax({
                url: "/Attendance/Verification",
                type: "POST",
                data: JSON.stringify({ EmpId: empId, EmployeeEnrolledStr: template }),
                processData: false,
                contentType: 'application/json',
                success: function (result) {
                    kendo.ui.progress($('.card'), false);
                    IsLoading = false;
                    if (result == "Verified Successfully") {
                        image.src = "../Content/Attendance/verified.png";
                    }
                    else {
                        image.src = "../Content/Attendance/notverified.png";
                    }
                    document.getElementById("msg").textContent = result;
                }
            });
        }
        else {
            var dia = $("#msgBox").data("kendoDialog");
            dia.title("Error");
            dia.content("Format Error");
            dia.open();
        }
    }
    else {
        var dia = $("#msgBox").data("kendoDialog");
        dia.title("Validation");
        dia.content("Please Select Employee");
        dia.open();
    }
}