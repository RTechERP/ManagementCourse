var examquestions = [];
var indexQuestion = 0;
var isSubmitSuccess = false;
var resultExamId = 0;
var listIdQuestion = [];
$(document).ready(function () {
    GetHistoryPractice();
    GetQuestion();
})


function GetHistoryPractice() {
    $.ajax({
        url: "/CourseExamPractice/GetHistoryResultPractice",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json',
        data: { courseExamId: courseExamId },
        success: function (response) {

            //console.log(response);
            var html = '';
            $.each(response, function (i, item) {
                var dateStart = moment(item.createdDate).isValid() ? moment(item.createdDate).format("DD/MM/YYYY HH:mm") : '';
                html += `<tr tabindex='0' onclick="GetResultPractice(${item.id})">
                            <th class="text-center">${dateStart}</th>
                            <th class="text-center">${item.goal}</th>
                            <th class="text-center">${item.practicePoints}</th>
                            <th class="text-center" style="background-color:${item.statusText == "Đạt" ? "Green" : (item.statusText == "Không đạt" ? "Red" : "Yellow")};
                                                                      color:${item.statusText == "Chưa chấm điểm" ? "black" : "white"}; ">${item.statusText}</th>
                        </tr>`;
            })
            $('#tbody_history_result').html(html);
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}

function GetResultPractice(id) {
    $.ajax({
        url: "/CourseExamPractice/GetResultPractice",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json',
        data: { courseResultId: id },
        success: function (response) {
            console.log(response)
            var html = '';
            $.each(response, function (i, item) {
                var dateStart = moment(item.createdDate).isValid() ? moment(item.createdDate).format("DD/MM/YYYY HH:mm") : '';
                html += `<tr class="text-wrap">
                            <th class="text-center">${item.stt}</th>
                            <th>${item.questionText}</th>
                            <th class="text-center">${item.point}</th>
                            <th>${item.note}</th>
                        </tr>`;
            })
            $('#tbody_practice_result').html(html);
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}

function GetQuestion() {
    $.ajax({
        url: "/CourseExamPractice/GetQuesstion",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json',
        data: { courseExamId: courseExamId },
        success: function (response) {
            var html = '';
            $.each(response, function (i, item) {
                listIdQuestion.push(item.id);
                html += `<tr>
                    <td class="p-1 text-center align-middle">${item.stt}</td>
                    <td class="p-1 ">
                        <a href="/CourseExamResult/QuestionDetails?questionId=${item.id}&courseId=${courseId}" target="_blank" class="link-color" style="white-space: pre-line;">${item.questionText}</a>
                    </td>
                </tr>`;
            })
            $('#tbody_exam_result').html(html);
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}


function DoExam() {
    let obj = {
        Id: 0,
        CourseExamId: parseInt(courseExamId),
        Note: ""
    };
    $.ajax({
        url: "/CourseExamPractice/CreatePracticeResult",
        type: "POST",
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(obj),
        success: function (result) {
            var dateCreate = moment(result.createdDate).isValid() ? moment(result.createdDate).format("DD/MM/YYYY HH:mm") : '';
            $("#time-exam").text(dateCreate);
            resultExamId = result.id;
            $("#modal_exam_practice_test").modal('show');
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}

function onSubmit() {
    Swal.fire({
        title: 'Bạn có chắc muốn nộp bài không?',
        showCancelButton: true,
        confirmButtonText: 'OK',
        overlay: true,
    }).then((result) => {
        if (result.isConfirmed) {
            $("#modal_exam_practice_test").modal('hide');
            GetHistoryPractice();
            onSave();
        }
    })

}

function onSave() {
    let objExamResult = {
        Id: parseInt(resultExamId),
        CourseExamId: parseInt(courseExamId),
        Status: 1,
        Note: ""
    };
    $.ajax({
        url: "/CourseExamPractice/ConfirmPractice",
        type: "POST",
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(objExamResult),
        success: function (response) {
            if (response == null) {
                alert("Không thể lưu bài thi!");
            }
            else {
                let arrData = [];
                listIdQuestion.forEach(e => {
                    let obj = {
                        Id: 0,
                        CourseExamResultId: parseInt(resultExamId),
                        CourseQuestionId: parseInt(e),
                        Point: 0,
                        Note: ""
                    };

                    arrData.push(obj);
                })

                console.log(arrData);
                $.ajax({
                    url: "/CourseExamPractice/CreateListExamValuate",
                    type: "POST",
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify(arrData),
                    success: function (result) {
                    },
                    error: function (error) {
                        alert(error.responseText);
                    }
                });
            }
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}

