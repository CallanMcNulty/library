$(document).ready(function(){
  i = 1;
  $(".append").click(function(event){
    $(".author").append('<br> <label for="author'+ i +'">Author Name:</label><input type="text" name="author' + i +'" value="">');

    $("#number-of-authors").val(i + 1);
    i ++;
  });

});
