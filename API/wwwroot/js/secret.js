$(document).ready(function() {
	var id = getUrlVars()["id"];
	var requestUrl = "http://localhost:5000/api/v1/secrets/"+id;
	
	$.ajax({
		type: "GET",
		url: requestUrl,
		cache: false,
		success: function(data) {
			console.log(data);
			$("meta[property='og:image']").attr("content", data.imageUrl);
			$("meta[property='og:description']").attr("content", data.description);
			$("#result").attr("src", data.imageUrl);
			$("#description").text(data.description);
		},
		error: function(data) {
			console.log(data.responseText);
		}
	});
	
	$('#share').click(function() {
		FB.ui({
			method: 'share',
			href: 'http://lapkisoft.me'
		},
		function (response) {
			if (response && !response.error_message) {
				console.log('successfully posted. Status id : ' + response.post_id);
			} else {
				console.log('Something went error.');
			}
		});
	});
	
	$('#try-again').click(function() {
		window.location.replace("/index.html");
	});
});

function getUrlVars()
{
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for(var i = 0; i < hashes.length; i++)
    {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}