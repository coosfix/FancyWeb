// var getauthtoken = "https://api.instagram.com/oauth/authorize/?client_id=b9b51863faca448e8f75bc1db4c6a4d7&redirect_uri=http://127.0.0.1:5500/book.html&response_type=token";
// $("#bb").click(function(){
//     let win = window.open(getauthtoken,"auth_window");
//     console.log(window.location);
// });

var token = '19072223196.b9b5186.45135071495644058c3c604d78701281',
    num_photos = 20;
$.ajax({
    url: "https://api.instagram.com/v1/users/self/media/recent",
    dataType: 'jsonp',
    data: { access_token: token, count: num_photos },
    success: function (datas) {
        console.log(datas);
        $.each(datas.data, function (key, value) {
            console.log("發文者姓名" + value.user.username);
            console.log("發文者頭像" + value.user.profile_picture);
            console.log("文章圖片" + value.images.standard_resolution.url);
            console.log("發文時間" + value.created_time);
            console.log("文章內容 " + value.caption.text);
            console.log("Likes " + value.likes.count);
            console.log("tags " + value.tags);
            console.log("回覆" + value.comments.count);
            console.log("文章id" + value.id);
        });
    },
    error: function (data) {
        console.log(data);
    }
});




