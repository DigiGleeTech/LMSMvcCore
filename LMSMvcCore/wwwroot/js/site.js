// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Document on load executary
$(() => {
    var preLoader = "Please wait";
    $('.InterestRate').prop('disabled', true);
    $('.amount').prop('disabled', true);

    $('.table').DataTable();
    $('#toggleDisplay').hide();
    $("#lblTotalAmount").hide();

})


//Select Lone type and asigned amout and interest 
$("#LoanTypeId").change(function () {
    var loanType = $(this).val();
    if (loanType == "Rice") {
        parseInt($('.amount').val(85780));
        $('.amount').prop('disabled', true);
        $(".InterestRate").val(2576.1);
    } else if (loanType == "Watanda") {
        parseInt($('.amount').val(20000));
        $('.amount').prop('disabled', true);
        parseInt($(".InterestRate").val(600));
    }
    else {
        $('.amount').val("");
        $(".InterestRate").val("");
        $('.amount').prop('disabled', false);
    }
});

//Calculate Interest
$(".amount").on("keyup", function () {
    var amount = $('.amount').val();
    var rate = (amount * 3) / 100;
    $("#InterestRate").val(rate);
});


// Check loan egibility
$(".process").on("click", function (e) {
    e.preventDefault();
    var dt = '';
    //var formData = $("#ApplyloanForm").serialize();
    var preLoad = ' <div class="spinner-border" role="status">< span class="visually-hidden" > Loading...</span ></div > ';
    var amount = $('.amount').val();
    var InterestRate = $('.InterestRate').val();
    var salary = $('#salary').val();
    if (amount != "" && InterestRate != null && salary != "") {
        $.ajax({
            url: "/Loans/CheckElibigility/",
            type: "GET",
            data: { Salary: salary, Amount: amount, Interest: InterestRate },
            dataType: 'Json',
            beforeSend: function () {
                $("#lblTotalAmount").html(preLoad);
                console.log("sending");
            },
            success: function (data) {
                $("#lblTotalAmount").html("");
                $.each(data, function (i, item) {
                    //Check if is eligible
                    if (item.amountPayedPerMonth >= salary) {
                        dt = '';
                        dt += '<label class="alert alert-danger"> ';
                        dt = 'You are not eligible to this loan. Please try again later';
                        dt += '</label>';
                        $("#lblTotalAmount").append(dt);
                        $("#lblTotalAmount").show();
                    } else {
                        dt += '<label class="alert alert-success"> You will paying ';
                        dt += item.amountPayedPerMonth + ' each month with the interest of ';
                        dt += item.interestRate + ' as 3% of the loan, which give the total amount of ';
                        dt += item.totalAmountToPay + ' that ';
                        dt += item.reminderPercent.toFixed(2) + '% of your salary within 4 month';
                        dt += '</label>';
                        $("#lblTotalAmount").append(dt);
                        $("#lblTotalAmount").show();
                        $('.process').hide();
                        $('#toggleDisplay').show();
                    }
                });
            },
            error: function (ex) {
                alert("error occour " + ex.error);
            }
        });
    } else {
        if (amount == "") {
            alert("Please amount can't be empty");
        }
        if (salary == "") {
            alert("Please salary can't be empty");
        }
        if (InterestRate == "") {
            alert("Please InterestRate can't be empty");
        }
    }


});


//Apply Loan
$("#btnApplyLoan").on("click", function (e) {
    e.preventDefault();
    var formData = $("#ApplyloanForm").serialize();

    $.ajax({
        url: "/Loans/Applyloan",
        type: "POST",
        data: formData,
        beforeSend: function () {
            alert("sending");
        },
        success: function (result) {
            alert("success");
        },
        error: function () {
            alert("error occour");
        }
    });
});


$(document).ready(function () {

    $('.amount').prop('disabled', true);
   
    $('#toggleDisplay').hide();



    //Validate PV Number
    $('#PVNumber').on("keyup", function () {
        var pv = ('#PVNumber').val();
        alert("Number + " + pv);
        var preLoad = ' <div class="spinner-border" role="status">< span class="visually-hidden" > Loading...</span ></div > ';
        if (pvNumber.length == 4) {
            $.ajax({
                url: "/Loans/ValidatePVNumber/",
                type: "GET",
                data: { pv: pv },
                /*dataType: 'Json',*/
                beforeSend: function () {
                    alert("sending");
                    $('#PVNSpan').html(preLoad);
                    console.log("sending");
                },
                success: function (data) {
                    $('#PVNSpan').html(data);
                },
                error: function (ex) {
                    alert("error occour " + ex.error);
                }
            })
        }
    });



    $("#btnMakePayment").on("click", function (e) {
        e.preventDefault();
        var id = $("#CustomerId").val();
        //alert(id);
        $.ajax({
            url: "/PayPoints/MakePayment",
            type: "POST",
            data: { id: id },
            beforeSend: function () {
                alert("sending");
            },
            success: function (result) {
                alert("Payment Success");
                $('#paymentView').html(result);
            },
            error: function (error) {
                alert("error occour: " + error.responseText + "  == " + error);
            }
        });
    });
    



});