<?php
$hostname = 'localhost';
$username = 'root';
$password = '';
$database = 'szwindel';

try {
    $dbh = new PDO('mysql:host=' . $hostname . ';dbname=' . $database, $username, $password);
    $dbh->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $checkTableSql = "SHOW TABLES LIKE 'users'";
    $tableExists = $dbh->query($checkTableSql)->rowCount() > 0;

    if (!$tableExists) {
        $createTableSql = "CREATE TABLE users (
            id INT AUTO_INCREMENT PRIMARY KEY,
            username VARCHAR(50) NOT NULL,
            password VARCHAR(50) NOT NULL,
            score INT DEFAULT 0
        )";

        $dbh->exec($createTableSql);
        echo "Table 'users' created successfully.";

        $insertUserSql = "INSERT INTO users (username, password) VALUES (:username, :password)";
        $stmt = $dbh->prepare($insertUserSql);
        $stmt->execute([':username' => 'Krzychu', ':password' => 'admin']);
        echo "\nUser 'Krzychu' created successfully.";
    } else {
        echo "Table 'users' already exists. No user was added.";
    }
} catch (PDOException $e) {
    echo '<h1>An error has occurred.</h1><pre>' . $e->getMessage() . '</pre>';
}
?>
