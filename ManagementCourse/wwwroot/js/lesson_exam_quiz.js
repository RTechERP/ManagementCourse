var examquestions = [];
var courseExamResultID = 0;
var indexQuestion = 0;
var isSubmitSuccess = false;
$(document).ready(function () {
    GetExamResult(courseExamId);
})

//Get danh sách kết quả
function GetExamResult(courseExamID) {
    $.ajax({
        url: "/CourseExamResult/GetExamResult",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json',
        data: { courseExamID: courseExamID },
        success: function (response) {
            //console.log(response);
            var html = '';
            $.each(response, function (i, item) {
                html += `<tr>
                            <td class="p-1">${item.NameCourse}</td>
                            <td class="p-1">${item.NameExam}</td>
                            <td class="text-end p-1">${item.TotalQuestion}</td>
                            <td class="text-end p-1">${item.TotalCorrect}</td>
                            <td class="text-end p-1">${item.TotalIncorrect}</td>
                            <td class="text-end p-1">${item.PercentageCorrect}</td>
                            <td class="text-center p-1">${moment(item.CreatedDate).format("DD/MM/YYYY HH:mm")}</td>
                            <td class="text-center p-1">${moment(item.UpdatedDate).format("DD/MM/YYYY HH:mm")}</td>
                        </tr>`;
            })

            $('#tbody_exam_result').html(html);
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}

function GetQuestionAnswerRight() {
    $.ajax({
        url: "/CourseExamResult/GetQuestionAnswerRight",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json',
        data: { courseExamResultId: courseExamResultID },
        success: function (response) {
            $('span.question-number').each(function () {
                var id = $(this).attr('id');
                var check = response.find(p => p.CourseQuestionId == id);
                if (!check) {
                    $(this).addClass('bg-danger');
                    $(this).addClass('text-white');
                }
            });
            clearInterval(time);
            $('.form-check-input').prop('disabled', true);
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}


//Sự kiện khi click làm bài thi
function onStart(status, examResultId) {

    var obj = {
        CourseExamId: parseInt(courseExamId)
    }

    if (status == 1) {
        $.ajax({
            url: "/CourseExamResult/CreateExamResult",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(obj),
            success: function (response) {

                if (parseInt(response) > 0) {
                    courseExamResultID = parseInt(response);
                    $('#modal_exam_test').modal('show');
                    isSubmitSuccess = false;
                    GetExamQuestion(lessonID, parseInt(response));
                    TimeCountDown();
                } else {
                    alert("Xảy ra lỗi khi tạo bài thi!");
                }
            },
            error: function (error) {
                alert(error);
            }
        });
    } else {
        courseExamResultID = examResultId;
        $('#modal_exam_test').modal('show');
        GetExamQuestion(lessonID, examResultId);
    }
}

//Get danh sách câu hỏi
function GetExamQuestion(lessonID, courseExamResultID) {
    $.ajax({
        url: "/CourseExamResult/GetExamQuestion",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json',
        data: {
            lessonID: lessonID,
            courseExamResultID: courseExamResultID
        },
        success: function (response) {
            examquestions = response.sort(() => Math.random() - 0.5);
            $.each(examquestions, function (i, item) {
                item.ExamAnswers.sort(() => Math.random() - 0.5);
            })
            addNumberQuestion(examquestions);
            var item = examquestions[0];
            $(`span[id="${item.ID}"]`).addClass('current-question');
            addContentQuestion(item);

            //examquestions = response;
            //addNumberQuestion(examquestions);
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}

//Sự kiện khi click next
function onNext() {
    AddResultDetail(false, true, false);
}

//sự kiện khi click prev
function onPrevios() {

    $('.current-question').removeClass('current-question');
    //Sửa lại điều kiện check
    if (examquestions.length > 0 && indexQuestion > 0) {
        indexQuestion--;
    }

    var item = examquestions[indexQuestion];
    addContentQuestion(item);
    $(`span[id="${item.ID}"]`).addClass('current-question');

}

//sự kiện khi click lưu
function onSave() {
    AddResultDetail(false, true, true);
}

//Sự kiện khi click chọn câu hỏi
function onChosenQuestion(event, id) {
    $('.current-question').removeClass('current-question');

    var currentAnswer = $(event.target).attr('id');

    var item = examquestions.find(x => x.ID == id);

    indexQuestion = examquestions.findIndex(x => x.ID == id);

    if (currentAnswer == item.ID) {
        $(event.target).addClass('current-question');
    }

    addContentQuestion(item)
}

//Get đáp án
function AddResultDetail(isSubmit, isNext, isSave) {
    var answers = [];
    $('.form-check-input').each(function () {

        var checked = $(this).is(':checked');
        //var checked = true;
        if (checked) {
            var obj = {
                AnswerText: $(`label[for="${$(this).attr('id')}"]`).html(),
                CourseQuestionId: parseInt($(this).attr('name')),
                CourseAnswerId: parseInt($(this).attr('id')),
                CourseExamResultId: courseExamResultID
            }
            answers.push(obj);

        }
    })

    $.ajax({
        url: "/CourseExamResult/CreateExamResultDetail",
        type: "POST",
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(answers),
        success: function (response) {
            var item = examquestions.find(x => x.ID == response.Item1);

            var index = examquestions.findIndex(x => x.ID == response.Item1);
            if (index >= 0) {
                $.each(examquestions[index].ExamAnswers, function (key, data) {
                    data.CourseAnswerChosenID = 0;
                })
            }

            if (item != null) {
                item.QuestionChosenID = response.Item1;
                $.each(response.Item2, function (key, data) {
                    //console.log(data);
                    var answer = item.ExamAnswers.find(x => x.ID == data);
                    answer.CourseAnswerChosenID = data;
                })
                addNumberQuestion(examquestions);
            }

            $('.form-check-input').prop('checked', false);

            if (isSave) {
                $('.current-question').removeClass('current-question');
                var item = examquestions[indexQuestion];
                addContentQuestion(item);
                $(`span[id="${item.ID}"]`).addClass('current-question');
            }

            if (isNext) {
                $('.current-question').removeClass('current-question');
                if (examquestions.length > 0 && indexQuestion < examquestions.length - 1) {
                    indexQuestion++;
                }
                var item = examquestions[indexQuestion];
                addContentQuestion(item);
                $(`span[id="${item.ID}"]`).addClass('current-question');
            }

            if (isSubmit) {
                isSubmitSuccess = true;
                GetQuestionAnswerRight();
                GetExamResult(courseExamId);
            }
        },
        error: function (error) {
            alert(error);
        }
    });

}

//Show câu hỏi lên giao diện
function addContentQuestion(item) {
    $("#current_question").html(indexQuestion + 1)

    //var inputname = item.ID;
    //var imageContainer = item.Image == '' ? '' : `<div class="d-flex justify-content-start">
    //                                                <img src="http://113.190.234.64:8083/api/Upload/Images/Courses/${item.Image}" style="width:100%;height:100%" />
    //                                            </div>`;

    //var htmlContent = `<input type="hidden" id="course_exam_question_id_${item.ID}" value="${item.ID}" />
    //                    <div class="card-body p-0 mb-3">
    //                        <div class="w-100">
    //                            <p class="text-dark fs-5 fw-bold">
    //                                ${item.QuestionText}
    //                            </p>
    //                        </div>
    //                        ${imageContainer}
    //                    </div>`;
    //var htmlAnswer = '';
    //$.each(item.ExamAnswers, function (i, item) {
    //    var checked = item.ID == item.CourseAnswerChosenID ? 'checked' : '';
    //    var disabled = isSubmitSuccess ? 'disabled' : '';
    //    var borderClass = i == 0 ? '' : 'border-top';

    //    htmlAnswer += `<div class="form-check form-check-exam-answer py-3 m-0 ${borderClass}">
    //                        <input class="form-check-input rounded-0" type="checkbox" name="${inputname}" id="${item.ID}" ${checked} ${disabled}>
    //                        <label class="form-check-label text-dark" for="${item.ID}">
    //                            ${item.AnswerText}
    //                        </label>
    //                    </div>`;

    //})

    //var html = htmlContent;
    //$('#exam_question_content').html(html);
    //$('#exam_content_answer').html(htmlAnswer);

    var inputname = item.ID;
    var htmlImage = item.Image == '' ? '' : ` <img src="http://113.190.234.64:8083/api/Upload/Images/Courses/${item.Image}" style="width:100%;height:100%" />`;
    var colQuestion = item.Image == '' ? 'col-12' : `col-6`;
    var htmlContent = `<div class="card-body py-1">
                            <input type="hidden" id="course_exam_question_id" value="0" />
                            <div class="card-body p-0">
                                <div class="w-100">
                                    <p class="text-dark fs-5 fw-bold" style="text-align:justify;">
                                        ${item.QuestionText}
                                    </p>
                                </div>
                            </div>
                        </div>`;
    var htmlAnswer = '';
    $.each(item.ExamAnswers, function (i, item) {
        var checked = item.ID == item.CourseAnswerChosenID ? 'checked' : '';
        var disabled = isSubmitSuccess ? 'disabled' : '';
        var borderClass = i == 0 ? '' : 'border-top';

        htmlAnswer += `<div class="form-check form-check-exam-answer py-3 m-0 ${borderClass}">
                            <input class="form-check-input rounded-0" type="checkbox" name="${inputname}" id="${item.ID}" ${checked} ${disabled}>
                            <label class="form-check-label text-dark" for="${item.ID}">
                                ${item.AnswerText}
                            </label>
                        </div>`;

    })
    $('#container_question_content').removeClass();
    $('#container_question_content').addClass(`${colQuestion}`);
    var html = htmlContent;
    $('#exam_question_content').html(html);
    $('#exam_content_answer').html(htmlAnswer);
    $('#image_question_container').html(htmlImage);

}

//Add number question
function addNumberQuestion(data) {
    if (isSubmitSuccess) {
        return;
    }
    var html = '';
    $.each(data, function (key, item) {
        var classSuccess = item.QuestionChosenID == item.ID ? 'question-success' : '';
        html += `<span class="question-number m-1 border border-dark ${classSuccess}" id="${item.ID}"
                            onclick="return onChosenQuestion(event,${item.ID});">${key + 1}</span>`;
    })

    $('#exam_question_navigation').html(html);

    $(".question-number").css({
        display: "inline-block",
        width: "30px",
        height: "30px",
        textAlign: "center",
        lineHeight: "30px",
        border: "1px solid #000",
        cursor: "pointer"
    });
}

//Sự kiện khi click nộp bài
function onSubmit() {

    Swal.fire({
        title: 'Bạn có chắc muốn nộp bài không?',
        showCancelButton: true,
        confirmButtonText: 'OK',
        overlay: true,
    }).then((result) => {
        if (result.isConfirmed) {
            AddResultDetail(true, false, false);
        }
    })

}

//Sự kiện khi click đóng
function onClose() {
    if (isSubmitSuccess) {
        $('#modal_exam_test').modal('hide');
    } else {
        onSubmit();
    }
}

var time = null;
//Sự kiện đếm ngược thời gian
function TimeCountDown() {
    testTime = parseInt(testTime);
    var remainingSeconds = testTime * 60;
    var step = 1000;
    time = setInterval(function () {
        if (testTime > 0) {
            remainingSeconds--;
        }
        document.getElementById("test_time").innerHTML = formatTime(remainingSeconds);
        if (remainingSeconds <= 0) {
            clearInterval(time);
            AddResultDetail(false, false, true);

            $('#btn_group_action_active').remove();
            $('#btn_group_action_disable').show();
            $('#btn_group_action_disable').addClass('d-flex');
        }
    }, step);
}

//Sự kiện khi load lại trang
window.onbeforeunload = (event) => {
    //$('#myModal').is(':visible');
    //console.log('modal is ', $('#modal_exam_test').is(':visible'));
    event.preventDefault();
    AddResultDetail(true, false, false);
};

//Format thời gian để hiển thị lên giao diện
function formatTime(time) {
    var minutes = Math.floor(time / 60);
    var seconds = Math.floor(time % 60);

    //console.log(time, minutes, seconds);

    var formattedTime = minutes.toString().padStart(2, '0') + ':' + seconds.toString().padStart(2, '0');

    return formattedTime;
}