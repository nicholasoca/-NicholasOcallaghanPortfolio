<?php
// Start the session
session_start();

// Unset all session variables
$_SESSION = array();

// Destroy the session
session_destroy();

// Redirect to the login page or any other page you prefer
header("Location: login.php");
exit();
?>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Logout - TextBookTreasures</title>
    <link rel="stylesheet" href="styles.css">
</head>

<body class="logout">
    <header>
        <img src="Logo.png" alt="TextBookTreasures Logo" id="logo">
        <h1>TextBookTreasures</h1>
        <div class="header-buttons">
            <a href="homepage.html"><button id="home-btn">Home</button></a>
        </div>
    </header>

    <main>
        <div class="logout-container">
            <h2>You have been successfully logged out.</h2>
            <a href="homepage.html"><button>Go to Homepage</button></a>
        </div>
    </main>

    <footer>
        <!-- Footer content if needed -->
        <img src="Bottom.png" alt="Bottom Image" class="full-width-image">
        <div class="newsletter-section">
            <p>Subscribe to our newsletter for updates</p>
            <div class="newsletter-form">
                <label for="email">Email:</label>
                <input type="email" id="email" name="email" placeholder="Your email...">
                <button>Subscribe</button>
            </div>
        </div>
    </footer>
</body>

</html>
