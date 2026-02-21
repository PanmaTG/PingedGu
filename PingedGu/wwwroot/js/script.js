// Add Post Uload Image
document.getElementById('addPostUrl').addEventListener('change', function(){
if (this.files[0] ) {
    var picture = new FileReader();
    picture.readAsDataURL(this.files[0]);
    picture.addEventListener('load', function(event) {
    document.getElementById('addPostImage').setAttribute('src', event.target.result);
    document.getElementById('addPostImage').style.display = 'block';
    });
    }
});


// Create Status Upload Image 
document.getElementById('createStatusUrl').addEventListener('change', function(){
if (this.files[0] ) {
    var picture = new FileReader();
    picture.readAsDataURL(this.files[0]);
    picture.addEventListener('load', function(event) {
    document.getElementById('createStatusImage').setAttribute('src', event.target.result);
    document.getElementById('createStatusImage').style.display = 'block';
    });
}
});


// Create Product Uload Image
document.getElementById('createProductUrl').addEventListener('change', function(){
if (this.files[0] ) {
    var picture = new FileReader();
    picture.readAsDataURL(this.files[0]);
    picture.addEventListener('load', function(event) {
    document.getElementById('createProductImage').setAttribute('src', event.target.result);
    document.getElementById('createProductImage').style.display = 'block';
    });
}
});







    