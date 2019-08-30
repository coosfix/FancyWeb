$('.menu-toggle > a').on('click', function (e) {
    e.preventDefault();
    $('#responsive-nav').toggleClass('active');
})
$(".dropdown").on("show.bs.dropdown", function () {
    $(".dropdown-menu").addClass("animation-in");
    setTimeout(() => {
        $(".dropdown-menu").removeClass("animation-in");
    }, 500)
});
try {
    var windowH = $(window).height() / 2;

    $(window).on('scroll', function () {
        if ($(this).scrollTop() > windowH) {
            $("#myBtn").addClass('show-btn-back-to-top');
        } else {
            $("#myBtn").removeClass('show-btn-back-to-top');
        }
    });

    $('#myBtn').on("click", function () {
        $('html, body').animate({
            scrollTop: 0
        }, 300);
    });
} catch (er) {
    console.log(er);
}
try {
    $(".animsition").animsition({
        inClass: 'fade-in',
        outClass: 'fade-out',
        inDuration: 1500,
        outDuration: 800,
        linkElement: '.animsition-link',
        loading: true,
        loadingParentElement: 'html',
        loadingClass: 'animsition-loading-1',
        loadingInner: '<div class="loader05"></div>',
        timeout: false,
        timeoutCountdown: 5000,
        onLoadEvent: true,
        browser: ['animation-duration', '-webkit-animation-duration'],
        overlay: false,
        overlayClass: 'animsition-overlay-slide',
        overlayParentElement: 'html',
        transition: function (url) {
            window.location.href = url;
        }
    });
} catch (er) {
    console.log(er);
}

$(".Line").click(() => {
    let url = location.href;
    let x = $(window).width() / 2 - 250;
    let y = $(window).height() / 2 - 250;
    window.open("https://social-plugins.line.me/lineit/share?url=" + url, "Line Share",
        `top=${y},left=${x},width=500,height=500`);
})


$(".twitter").click(() => {
    let url = location.href;
    let x = $(window).width() / 2 - 250;
    let y = $(window).height() / 2 - 250;
    window.open(`https://twitter.com/intent/tweet?hashtags=Fancy&text=分享這項商品到你的朋友圈吧!&url=${url}&via=twitterdev`, "Tweet Share",
        `top=${y},left=${x},width=500,height=500`);
})
var lastScroll = 0;
$(window).scroll(function () {
    let scrollNow = $(this).scrollTop();
    if (lastScroll > scrollNow) {
        $("#navigation").removeClass("active");
    } else {
        $("#navigation").addClass("active");
    }
    lastScroll = scrollNow;
});