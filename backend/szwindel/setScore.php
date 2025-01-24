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
        $scoreToAdd = isset($_GET['score']) ? (int)$_GET['score'] : 0;

        if ($userId > -1 && $scoreToAdd !== 0) {
            $sth = $dbh->prepare('UPDATE users SET score = score + :score WHERE id = :id');
            $sth->bindParam(':id', $userId, PDO::PARAM_INT);
            $sth->bindParam(':score', $scoreToAdd, PDO::PARAM_INT);
            $sth->execute();

            if ($sth->rowCount() > 0) {
                echo "User's score updated successfully.";
            } else {
                echo "<h1>An error has occurred: no user with this ID.</h1>";
            }
        } else {
            echo "<h1>An error has occurred: incorrect ID or score.</h1>";
        }
    } catch (PDOException $e) {
        echo '<h1>An error has occurred.</h1><pre>' . $e->getMessage() . '</pre>';
    }
?>