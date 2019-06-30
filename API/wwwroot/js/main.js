$(document).ready(function(){
	$('form').submit(function() {
		var $input = $('input[type="file"]');
		var fd = new FormData;
		fd.append('imageFile', $input.prop('files')[0]);
		
		$.ajax({
			type: "POST",
			url: $(this).attr('action'),
			data: fd,
			processData: false,
			contentType: false,
			cache: false,
			success: function(dataUrl) {
				console.log(dataUrl);
						
				$.ajax({
					type: "POST",
					url: "/api/v1/secrets/",
					data: '{"imageUrl": "' + dataUrl + '"}',
					headers: { 
						'Accept': 'application/json',
						'Content-Type': 'application/json' 
					},
					dataType: 'json',
					cache: false,
					success: function(secret) {
						showResult(secret.id);
					},
					error: function(data) {
						console.log(data.responseText);
					}
				});
			},
			error: function(data) {
				console.log(data.responseText);
			}
		});
		return false;
	});
});

function showResult(id) {
	window.location.replace("/secrets.html?id="+id);
}