<?php
	$servername = "localhost";
	$username = "root";
	$password = "";
	$dbName = "highscore";
	
	$conn = new mysqli($servername, $username, $password, $dbName);
	
	if (!$conn) {
		die("Connection faild".mysqli_connect_error());
	}
	
	$sql = "SELECT * FROM `highscore` WHERE 1";
	$result = mysqli_query($conn, $sql);
	
	if (mysqli_num_rows($result) > 0) {
		while($row = mysqli_fetch_assoc($result)) {
			echo "ID:".$row['playerID']."|Name:".$row['name']."|Score:".$row['score'].";";
		}
	}
?>