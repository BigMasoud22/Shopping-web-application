function DeleteProduct(Url) {
    Swal.fire({
        title: 'مطمعن هستید؟',
        text: "این عمل غیر قابل بازگشت هست!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'آره حذفش کن'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: Url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        $('#productTbl').DataTable().ajax.reload();
                        Swal.fire(
                            'حذف شد',
                            'محصول با موفقیت حذف شد',
                            'success'
                        )
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: 'اوه...',
                            text: 'یه اتفاقی افتاده',
                        })
                    }
                }
            })
        }
        else {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Something went wrong!',
            })
        }
    })
}
function DeleteCategory(Url) {
    Swal.fire({
        title: 'از حذف دسته مطمیعن هستید؟',
        text: "این عمل غیر قابل بازگشت است!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'حذفش کن'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: Url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        $('#categoryTbl').DataTable().ajax.reload();
                        Swal.fire(
                            'موفقیت آمیز!',
                            'دسته موزد نظر با موفقیت حذف شد',
                            'success'
                        )
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: 'اوپس...',
                            text: 'خطایی رخ داده',
                        })
                    }
                }
            })
        }
    })
}
function toggleMenu(event, element ,listId) {
    event.preventDefault();

    var menu = document.getElementById(listId);
    var isOpen = menu.style.display === "block";

    if (isOpen) {
        menu.style.display = "none";
        element.classList.remove("open");
    } else {
        menu.style.display = "block";
        element.classList.add("open");
    }
}
function ChangePasswordSuccessNotification() {
    Swal.fire({
        position: 'center',
        icon: 'success',
        title: 'رمز عبور با موفقیت تغییر کرد',
        showConfirmButton: false,
        timer: 4000
    })
}
function Logout(url) {
    Swal.fire({
        title: 'مطمئن هستید؟',
        text: "این عمل بازگشتی ندارد",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'آره خارج شو!',
        cancelButtonText:'نه بیخیال'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                success: function (data) {
                    if (true) {
                        window.location.href = '/Main/Home/Index';
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: 'ای وای...',
                            text: 'خطایی رخ داده!',
                        })
                    }
                }
            })
        }
    })
}
function redirectToAreaAction(area, controller, action) {
    window.location.href = '/' + area + '/' + controller + '/' + action;
}
function LoginAlert() {
    var returnUrl = window.location.href;

    Swal.fire({
        title: 'متاسفیم:( ',
        text: "برای انجام این کار باید وارد اکانتت بشی",
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'آره بزن بریم!',
        cancelButtonText: "نه بیخیال"
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = '/Identity/Account/Login?returnUrl=' + encodeURIComponent(returnUrl);
        }
    });
}
function deleteProduct(url, productId) {
    Swal.fire({
        title: 'مطمعن هستید؟',
        text: "این عمل غیر قابل بازگشت هست!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'آره حذفش کن'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                data: { cartId: productId },
                success: function (data) {
                    if (data.success) {
                        reloadPage();
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: 'اوه...',
                            text: 'یه اتفاقی افتاده',
                        })
                    }
                },
            })
        }
    })
}

var categoryTable;
var productTable;
$(document).ready(function () {
    LoadCategoryDataTable();
    LoadProductDataTable();
});

function LoadCategoryDataTable() {
    if ($.fn.DataTable.isDataTable('#categoryTbl')) {
        categoryTable.destroy();
    }

    categoryTable = $('#categoryTbl').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetCategories",
        },
        "columns": [
            { "data": "name", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div>
                        <a type="button" class="btn btn-outline-info" href="/Admin/Product/UpsertCategory?CategoryId=${data}">ویرایش</a>
                        <a type="button" class="btn btn-primary" onClick=DeleteCategory('/Admin/Product/DeleteCategory?CategoryId=${data}')>حذف</a>
                    </div> `
                },
                "width": "15%"
            }
        ]
    });
}

function LoadProductDataTable() {
    if ($.fn.DataTable.isDataTable('#productTbl')) {
        productTable.destroy();
    }

    productTable = $('#productTbl').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetProducts",
        },
        "bDestroy": true,
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "price", "width": "15%" },
            { "data": "sellCount", "width": "15%" },
            {
                "data": "isAvaliable",
                "width": "15%",
                "render": function (data) {
                    return data ? "هست" : "نیست";
                }
            },
            { "data": "categoryName", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div>
                        <a type="button" class="btn btn-outline-info" href="/Admin/Product/UpsertProduct?ProductId=${data}">ویرایش</a>
                        <a type="button" class="btn btn-primary" onClick=DeleteProduct('/Admin/Product/DeleteProduct?ProductId=${data}')>حذف</a>
                    </div> `
                },
                "width": "15%"
            }
        ]
    });
}
function reloadPage() {
    window.location.reload();
}
