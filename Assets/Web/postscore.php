<?php
	$servername = "localhost";
	$username = "root";
	$password = "";
	$dbName = "highscore";
	
	$conn = new mysqli($servername, $username, $password, $dbName);
	
	$usernamePost = $_POST["usernamePost"];
	$scorePost = $_POST["scorePost"];
	
	if (!$conn) {
		die("Connection faild".mysqli_connect_error());
	}
	
	$sql = "INSERT INTO highscore (name, score) VALUES ('{$usernamePost}', {$scorePost});";
	$result = mysqli_query($conn, $sql);
	
	if (!$result) {
		echo("Request error!");
	}
?>