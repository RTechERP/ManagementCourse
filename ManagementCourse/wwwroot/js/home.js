$(document).ready(function () {
    //GetAllCourse(0, '');

    //$('#value_search').keypress(function () {
    //    GetAllCourse($(this).val());
    //});
    //$("#value_search").on("input", function () {
    //    if ($(this).val().length == 0) {
    //        GetAllCourse($(this).val());
    //    }
    //});

    $(`#menu_link_items_${courseCatalogID}`).parent().addClass("active");
    $('#menu_link_items_7').click();

    console.log('catalogType: ', catalogType);

    $('#menu_item_basic').removeClass('open');
    $('#menu_item_advance').removeClass('open');

    if (catalogType == 1) {
        $('#menu_item_basic').addClass('open');
        //$('#menu_item_advance').removeClass('open');
    } else if (catalogType == 2) {
        $('#menu_item_advance').addClass('open');
        //$('#menu_item_basic').removeClass('open');
    }

    //
});


//function CheckNullCourse(id, event) {
//    event.preventDefault();

//    $.ajax({
//        url:'/Home/CheckNullCourse',
//        type: 'POST',
//        dataType: 'json',
//        data: {
//            id: id,
//        },      
//        success: function (result) {
//            if (result == 0) {
//                MessageWarning("Hiện tại chưa có bài học nào !");
//            } else {
//                window.location = `/Lesson/Index?courseId=${id}&lessionId=0`;
//            }

//        },

//        error: function () {
//            MessageError("Hiện tại chưa có bài học nào! ");
//        }
//    });
//}

//Get danh sách khoá học
function GetAllCourse(keyword) {

    let data = {
        id: courseCatalogID,
        filterText: keyword,
        departmentID: departmentID,
        catalogType: catalogType
    };

    console.log('data GetAllCourse: ', data);

    var html = '';
    var title = '';
    $.ajax({
        url: '/Course/GetListCourse',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        data: {
            id: courseCatalogID,
            filterText: keyword,
            departmentID: departmentID,
            catalogType: catalogType
        },

        success: function (result) {
            $.each(result, function (key, item) {

                var classStatus = item.DeleteFlag == 1 ? 'text-success' : 'text-danger';
                var classEvaluate = item.Evaluate == 1 ? 'text-success' : 'text-danger';
                html += `<div class="card mb-3">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-8">
                                        <p class="m-0 fw-bold ${classStatus}">${item.DeleteFlagText}</p>
                                        <h5 class="card-title m-0 text-uppercase fw-bold">
                                            <a href="/Lesson/Index?courseId=${item.ID}&lessionId=0" class="px-0">${item.NameCourse}</a>
                                        </h5>
                                        <p class="m-0 text-dark">${item.Instructor}</p>
                                    </div>

                                    <div class="col-4">
                                        <div class="demo-inline-spacing m-0 d-flex justify-content-end">
                                            <ul class="list-group m-0">
                                                <li class="list-group-item d-flex justify-content-start align-items-center p-0 border-0">
                                                    <p class="m-0">Bài học: <span>${item.TotalHistoryLession}/${item.NumberLesson}</span></p>

                                                </li>
                                                <li class="list-group-item d-flex justify-content-start align-items-center p-0 border-0">
                                                    <p class="m-0">Điểm thi: <span>${item.PercentageCorrect}</span></p>
                                                </li>
                                                <li class="list-group-item d-flex justify-content-start align-items-center p-0 border-0">
                                                    <p class="m-0">Đánh giá: <span class="${classEvaluate}">${item.EvaluateText}</span></p>
                                                </li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>

                                <a href="/Lesson/Index?courseId=${item.ID}&lessionId=0" class="btn btn-primary btn-sm">Xem chi tiết</a>
                            </div>
                        </div>`;

                title = id == 0 ? `danh sách khoá học ${item.DepartmentName}` : `danh sách khoá học ${item.NameCourseCatalog}`;
            });

            $('#title_course').text(title);
            $('#list_course_items').html(html);

        },

        error: function () {
            MessageError("Error loading data! Please try again.");
        }
    });
}

//Sự kiệ tìm kiếm
function onInputSearch() {
    var keyword = $('#value_search').val();
    GetAllCourse(0, keyword);

}

//Sự kiện chọn khoá học
function onClickMenu(event, id, keyword) {
    $('.menu-item').removeClass('active');
    $(event.target).parent().addClass('active');

    GetAllCourse(id, keyword)
}
