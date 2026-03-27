

$(document).ready(function () {

    $('input[type=radio]').change(function () {
        var questionId = $(this).attr('data-id');
        var errorSpan = $(this).closest('.question').find('.error-message');
        if ($(this).is(':checked')) {
            errorSpan.hide();
        } else {
            errorSpan.show();
        }
    });
    var currentQuestionIndex = 0;
    var courseExamId = window.courseExamID;
    var resultID = window.resultID;

    $.ajax({
        url: "/CourseExam/CheckExamCompletion",
        type: "GET",
        data: { resultID: resultID },
        success: function (response) {
            if (response.isCompleted) {
                // Exam has been completed, display the exam result and the "Restart" button
                console.log(response);
                var percent = response.percent;
                var resultHtml = `
                    <div class="card col-md-12">
                                          <div class="card-header">
                                            <h3 class="card-title">Kết quả thi</h3>
                                          </div>
                                          <div class="card-body">
                                            <h3>Số câu đúng: <span id="num-correct-answers">${response.totalCorrect}/${response.totalCorrect + response.totalInCorrect}</span></h3>
                                          </div>
                                      </div>
                                    <button class="btn btn-primary mt-3" id="btn-restart-exam">Làm lại bài thi</button>
                `;

                // Clear the content within the row
                $(".row.mb-3").empty().html(resultHtml);
            } else {
                // Exam has not been completed, continue with the normal flow of loading questions
                loadQuestions();
            }
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseText);
        }
    });

    //Lấy ra câu hỏi
    function loadQuestions() {
        $.ajax({
            url: '/CourseExam/GetQuestions',
            type: 'GET',
            data: { courseExamId: courseExamId },
            success: function (response) {
                console.log(response);
                var questionContainer = $('#question-container');
                questionContainer.empty();

                // Shuffle the course questions randomly
                var shuffledQuestions = response.courseQuestion.sort(() => Math.random() - 0.5);

                $.each(shuffledQuestions, function (index, question) {
                    var questionHtml = `<div class="question" data-question-index="${index}">
                                        <h4>${question.questionText}</h4>
                                        <input value="${question.id}" id="question-id-${index}" style="display: none;">`;

                    // Get the answers for the current question
                    var questionAnswers = response.courseAnswer.filter(answer => answer.courseQuestionId === question.id);

                    // Shuffle the answers randomly
                    var shuffledAnswers = questionAnswers.sort(() => Math.random() - 0.5);

                    $.each(shuffledAnswers, function (answerIndex, answer) {
                        questionHtml += `
                                        <div class="form-check form-check-inline">
                                        <input value="${answer.id}" id="answer-id-${answer.id}" style="display: none;">
                                        <input class="form-check-input" type="checkbox" name="question_${index}" id="q_${answer.id}" value="${answer.id}" data-id="${question.id}">
                                        <label class="form-check-label" for="q_${answer.id}">${answer.answerText}</label> 
                                    </div><br>`;
                    });

                    //questionHtml += '<hr/></div>';

                    questionContainer.append(questionHtml);
                });


                $(".question").hide();
                $(".question[data-question-index='0']").show();
                loadSelectedAnswers(currentQuestionIndex, resultID);
                //Load ra mấy cái ô hiện câu hỏi bên phải
                var questionNavigation = $('#question-navigation');
                questionNavigation.empty();

                for (var i = 0; i < response.courseQuestion.length; i++) {
                    var questionNumberHtml = `<span class="question-number m-1 border border-dark" data-question-index="${i}">${i + 1}</span>`;
                    questionNavigation.append(questionNumberHtml);
                }
                $('.question-number[data-question-index="0"]').addClass('current-question');

                $(".question-number").css({
                    display: "inline-block",
                    width: "30px",
                    height: "30px",
                    textAlign: "center",
                    lineHeight: "30px",
                    border: "1px solid #000",
                    cursor: "pointer"
                });

                $('.question-number').click(function () {
                    var clickedQuestionIndex = $(this).data('question-index');
                    $('.question-number').removeClass('current-question');
                    $(this).addClass('current-question');
                    currentQuestionIndex = clickedQuestionIndex;
                    showQuestion(currentQuestionIndex);
                    loadSelectedAnswers(currentQuestionIndex, resultID);
                });


                $('.question-number').click(function () {
                    var questionIndex = $(this).data('question-index');
                    showQuestion(questionIndex);
                });
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    //Ẩn hiện câu hỏi
    function showQuestion(questionIndex) {
        $('.question').hide();
        $('.question[data-question-index="' + questionIndex + '"]').show();
    }


    //Add màu cho mấy cai ô
    function updateQuestionView() {
        $('.question-number').removeClass('current-question');
        $('.question-number[data-question-index="' + currentQuestionIndex + '"]').addClass('current-question');
        showQuestion(currentQuestionIndex);
    }

    $(".question[data-question-index='0']").show();

    //Next
    $("#next-button").click(function () {
        var currentQuestion = $(".question[data-question-index='" + currentQuestionIndex + "']");
        var nextQuestion = $(".question[data-question-index='" + (currentQuestionIndex + 1) + "']");

        if (nextQuestion.length > 0) {
            saveSelectedAnswers(currentQuestionIndex);
            currentQuestion.hide();
            nextQuestion.show();
            currentQuestionIndex++;
            loadSelectedAnswers(currentQuestionIndex, resultID);
            updateQuestionView();
        }
    });

    //Prev
    $("#prev-button").click(function () {
        var currentQuestion = $(".question[data-question-index='" + currentQuestionIndex + "']");
        var prevQuestion = $(".question[data-question-index='" + (currentQuestionIndex - 1) + "']");

        if (prevQuestion.length > 0 && currentQuestionIndex > 0) {
            saveSelectedAnswers(currentQuestionIndex);
            currentQuestion.hide();
            prevQuestion.show();
            currentQuestionIndex--;
            loadSelectedAnswers(currentQuestionIndex, resultID);
            updateQuestionView();
        }
    });

    //Save 
    function saveSelectedAnswers(questionIndex) {
        var selectedAnswers = $("input[name=question_" + questionIndex + "]:checked").map(function () {
            return this.value;
        }).get();

        saveAnswers(questionIndex, selectedAnswers, resultID);
    }

    //Load đáp án các câu hỏi đã làm
    function loadSelectedAnswers(questionIndex, resultID) {
        var questionId = parseInt($('#question-id-' + questionIndex).val());

        $.ajax({
            type: "POST",
            url: "/CourseExam/GetQuestionAnswers",
            data: { questionId: questionId, courseExamResultId: resultID },
            success: function (previousAnswers) {
                if (previousAnswers !== null) {
                    for (var i = 0; i < previousAnswers.length; i++) {
                        var previousAnswer = previousAnswers[i];
                        $("input[name=question_" + questionIndex + "][value=" + previousAnswer.courseAnswerId + "]").prop("checked", true);
                    }
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }


    function saveAnswers(questionIndex, selectedAnswers, resultID) {
        var answerData = [];

        for (var i = 0; i < selectedAnswers.length; i++) {
            var answer = {
                CourseQuestionID: parseInt($('#question-id-' + questionIndex).val()),
                CourseAnswerID: selectedAnswers[i],
                CourseExamResultID: resultID
            };
            answerData.push(answer);
        }

        $.ajax({
            type: "POST",
            url: "/CourseExam/SaveQuestionAnswers",
            data: JSON.stringify(answerData),
            contentType: "application/json",
            success: function (response) { },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    $("#btn-submit-exam").click(function () {
        var isValid = true;
        $('.question').each(function () {
            var questionId = $(this).find('input[type=radio]:checked').attr('data-id');
            if (!questionId) {
                $(this).find('.error-message').show();
                isValid = false;
            } else {
                $(this).find('.error-message').hide();
            }
        });
        if (!$('.error-message:visible').length) {
            isValid = true;
        } else {
            alert('Bạn chưa chọn đáp án cho tất cả các câu hỏi!');
        }

        if (isValid) {
            saveSelectedAnswers(currentQuestionIndex);
            $.ajax({
                type: "POST",
                url: "/CourseExam/SubmitExam",
                data: { resultID: resultID },
                success: function (response) {
                    // Update the exam result based on the response
                    var numCorrectAnswers = response.numCorrectAnswers;
                    var numIncorrectAnswers = response.numIncorrectAnswers;

                    // Clear the content within the row
                    $(".row.mb-3").empty();

                    var resultHtml = `<div class="card col-md-12">
                                          <div class="card-header">
                                            <h3 class="card-title">Kết quả thi</h3>
                                          </div>
                                          <div class="card-body">
                                            <h3>Số câu đúng: <span id="num-correct-answers">${numCorrectAnswers}/${numCorrectAnswers + numIncorrectAnswers}</span></h3>
                                          </div>
                                      </div>
                                    <button class="btn btn-primary mt-3" id="btn-restart-exam">Làm lại bài thi</button>`;
                    $(".row").html(resultHtml);

                    console.log(response);
                },
                error: function (xhr, status, error) {
                    console.log(xhr.responseText);
                }
            });
        }
    });


    $(document).on('click', '#btn-restart-exam', function () {
        $.ajax({
            type: "POST",
            url: "/CourseExam/RetakeExam",
            data: { resultID: resultID },
            success: function (response) {
                // Reload the page to start the exam again
                window.location.reload();
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    });

});


function CheckHistoryLess(event, id) {

    event.preventDefault();
    this.blur();
    Swal.fire({
        title: 'Bạn có muốn chắc chắn đổi trạng thái ?',
        showCancelButton: true,
        confirmButtonText: 'OK',
        overlay: true,
    }).then((result) => {
        if (result.isConfirmed) {


            $.ajax({
                url: '/Lesson/CheckHistoryLesson',
                type: 'POST',
                dataType: 'json',
                data: {
                    lessonId: id,
                },
                traditional: true,
                success: function (result) {
                    if (result == 1) {
                        $('#historyCheckbox_' + id).prop('checked', true); // Thay đổi giá trị checked thành true
                        if ($('#id_lesson').val() == id) {
                            $('#title-check-less').text('Đã học');
                        }
                    } else if (result == 0) {
                        $('#historyCheckbox_' + id).prop('checked', false);
                        if ($('#id_lesson').val() == id) {
                            $('#title-check-less').text('Chưa học');
                        }

                    } else {
                        MessageError("Lỗi ");
                    }

                },

                error: function () {
                    MessageError("Lỗi ");
                }
            });

        }
    })
}
