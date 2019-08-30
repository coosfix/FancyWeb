$(document).ready(function () {
    $('#Membertable').DataTable({
        "bLengthChange": true,
        "lengthMenu": [5, 10, 15],
        "language": {
            "processing": "處理中...",
            "loadingRecords": "載入中...",
            "lengthMenu": "顯示 _MENU_ 項結果",
            "zeroRecords": "沒有符合的結果",
            "info": "顯示第 _START_ 至 _END_ 項結果，共 _TOTAL_ 項",
            "infoEmpty": "顯示第 0 至 0 項結果，共 0 項",
            "infoFiltered": "(從 _MAX_ 項結果中過濾)",
            "infoPostFix": "",
            "search": "搜尋",
            "paginate": {
                "first": "第一頁",
                "previous": "<span aria-hidden='true'>&laquo;</span>",
                "next": "<span aria-hidden='true'>&raquo;</span>",
                "last": "最後一頁"
            },
            "aria": {
                "sortAscending": ": 升冪排列",
                "sortDescending": ": 降冪排列"
            }
        }
    });
    $('#table_id2').DataTable({
        "bLengthChange": true,
        "lengthMenu": [5, 10, 15],
        "language": {
            "processing": "處理中...",
            "loadingRecords": "載入中...",
            "lengthMenu": "顯示 _MENU_ 項結果",
            "zeroRecords": "沒有符合的結果",
            "info": "顯示第 _START_ 至 _END_ 項結果，共 _TOTAL_ 項",
            "infoEmpty": "顯示第 0 至 0 項結果，共 0 項",
            "infoFiltered": "(從 _MAX_ 項結果中過濾)",
            "infoPostFix": "",
            "search": "搜尋",
            "paginate": {
                "first": "第一頁",
                "previous": "<span aria-hidden='true'>&laquo;</span>",
                "next": "<span aria-hidden='true'>&raquo;</span>",
                "last": "最後一頁"
            },
            "aria": {
                "sortAscending": ": 升冪排列",
                "sortDescending": ": 降冪排列"
            }
        }
    });
    $('.tabbar ul li a').on('click', function (e) {
        var self = $(this);
        $("#navBarHover").css('width', self.parent().width());
        $("#navBarHover").css('left', self.parent().position().left);
    });
    var ccc = $(".main-mamn>div:last-child");
    var ddd = $('.main-mamn>div:first-child');
    ccc.css("top", `${ddd.height()}px`);
    $(".tabbar ul li").click(function () {
        let clickindex = $(this).index();
        let domain = $(".main-mamn>div").toArray();
        let domainre = $(".main-mamn>div").toArray().reverse();
        for (var i = 0; i < domain.length; i++) {
            let divobj = $(domain[i]);
            let thistop = $(domainre[i]).position().top;
            divobj.css("top", `${thistop}px`);
        }
    });

});
