//var getauthtoken = "https://api.instagram.com/oauth/authorize/?client_id=792d1319f95a49529408600f12165515&redirect_uri=https://localhost:44395/HomePage&response_type=token";
// $("#bb").click(function(){
//     let win = window.open(getauthtoken,"auth_window");
//     console.log(window.location);
// });

//var token = '19072223196.b9b5186.45135071495644058c3c604d78701281',
var token = '19072223196.792d131.e00ffb369ef84a0983e5fea1c0f9d093',
    num_photos = 20,
    all_ig_data;
$.ajax({
    url: "https://api.instagram.com/v1/users/self/media/recent",
    dataType: 'jsonp',
    data: { access_token: token, count: num_photos },
    success: function (datas) {
        //儲存全部文章資料
        all_ig_data = datas.data;
        console.log(datas);
        let Frag = $(document.createDocumentFragment());
        $.each(all_ig_data, function (key, value) {
            let card = $("<div data-igdata>").attr("data-igdata", value.id).addClass("card");
            let card_2 = $("<div>").addClass("ig-img bg-cover").css({
                "background-image": `url(${ value.images.standard_resolution.url })`,
                "height": "200px" 
            });
            card.append(card_2);
            Frag.append(card);
            //console.log("發文者姓名" + value.user.username);
            //console.log("發文者頭像" + value.user.profile_picture);
            //console.log("文章圖片" + value.images.standard_resolution.url);
            //console.log("發文時間" + value.created_time);
            //console.log("文章內容 " + value.caption.text);
            //console.log("Likes " + value.likes.count);
            //console.log("tags " + value.tags);
            //console.log("回覆" + value.comments.count);
            //console.log("文章id" + value.id);
        });
        $(".slider-body").html(Frag);
        $('.slider-body').slick({
            slidesToShow: 6,
            slidesToScroll: 6,
            arrows: false,
            responsive: [
                {
                    breakpoint: 768,
                    settings: {
                        infinite: true,
                        slidesToShow: 1,
                        slidesToScroll: 1,
                        adaptiveHeight: true
                    }
                }
            ]
        });
    },
    error: function (data) {
        console.log(data);
    }
});




