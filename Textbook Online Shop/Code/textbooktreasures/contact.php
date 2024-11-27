<?php
$conn = mysqli_connect("localhost", "root", "", "bookstore");

// Check connection
if (!$conn) {
    die("Connection failed: " . mysqli_connect_error());
}

if ($_SERVER["REQUEST_METHOD"] == "POST") {
    $contactEmail = mysqli_real_escape_string($conn, $_POST['contactEmail']);

    // Insert data into 'contact' table
    $insertQuery = "INSERT INTO contact (email) VALUES ('$contactEmail')";

    if (mysqli_query($conn, $insertQuery)) {
        echo "Thank you for submitting your email!";
    } else {
        echo "Error: " . $insertQuery . "<br>" . mysqli_error($conn);
    }
}
?>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>TextBookTreasures - Contact Us</title>
    <link rel="stylesheet" href="styles.css">
</head>

<body>
    <header>
        <img src="images\Logo.png" alt="TextBookTreasures Logo" id="logo">
        <h1>TextBookTreasures</h1>
        <div class="header-buttons">
            
            <a href="cart.php"><button id="cart-btn">Cart</button></a>
            <a href="login.php"><button id="login-register-btn">Login/Register</button></a>
        </div>
    </header>

    <nav>
        <ul>
            <li><a href="homepage.php">Home</a></li>
            <li><a href="buyBook.php">Buy a Book</a></li>
            <li><a href="sellBook.php">Sell a Book</a></li>
            <li><a href="contact.php">Contact Us</a></li>
        </ul>
        <div id="auth-links">
            <a href="#">Register</a>
            <a href="#">Login</a>
        </div>
    </nav>

    <main>
        <!-- Body content for Contact Us page -->
        <h2>Contact Us</h2>
        <div class="contact-info">
            <p>Phone: 012 345 6789</p>
            <p>Email: info@textbooktreasures.co.za</p>
        </div>

        <p>OR</p>
        <p>Insert your email, and we will contact you!</p>

        <form action="contact.php" method="post" class="contact-form">
            <label for="contactEmail">Your Email:</label>
            <input type="email" id="contactEmail" name="contactEmail" placeholder="Your email..." required>
            <button type="submit">Submit</button>
        </form>
    </main>

    <footer>
        <!-- Add footer content if needed -->
        <img src="images\Bottom.png" alt="Bottom Image" class="full-width-image">
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
