<?php
    // Configuration
    $hostname = 'localhost';
    $username = 'root';
    $password = '';
    $database = 'szwindel';

    try {
        $dbh = new PDO('mysql:host=' . $hostname . ';dbname=' . $database, $username, $password);
        $dbh->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

        $userId = isset($_GET['id']) ? (int)$_GET['id'] : -1;

        if ($userId > -1) {
            $sth = $dbh->prepare('SELECT username FROM users WHERE id = :id');
            $sth->bindParam(':id', $userId, PDO::PARAM_INT);
            $sth->execute();

            $user = $sth->fetch(PDO::FETCH_ASSOC);

            if ($user) {
                echo $user['username'];
            } else {
                echo "<h1>An error has occurred: no user with this ID.</h1>";
            }
        } else {
            echo "<h1>An error has occurred: incorrect ID.</h1>";
        }
    } catch (PDOException $e) {
        echo '<h1>An error has occurred.</h1><pre>' . $e->getMessage() . '</pre>';
    }
?>