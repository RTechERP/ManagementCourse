//var _URL = "https://localhost:44337/"; 
//var _URL = "https://192.168.1.2/";

//Hiển thị sweet alert lỗi
function MessageError(text) {
    new swal({
        title: "Thông báo",
        icon: 'error',
        text: text,
        button: false,
        //timer: 2000
    });
}

//Hiển thị sweet alert thành công
function MessageSucces(text) {
  new  swal({
        title: 'Thông báo',
        text: text,
        button: false,
        icon: 'success',
        timer: 1000
    });
}

//Hiển thị sweet alert cảnh báo
function MessageWarning(text, timer) {
   new swal({
        title: 'Thông báo',
        text: text,
        button: false,
        icon: 'warning',
        timer: timer
    });
}