<!-- homepage.php -->

<?php
// Assuming you've established a database connection
$conn = mysqli_connect("localhost", "root", "", "bookstore");

$sql = "SELECT * FROM books";
$result = mysqli_query($conn, $sql);
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
        <!-- Buy and Sell buttons -->
        <div class="cta-buttons">
            <a href="buyBook.php"><button class="buy-button">Buy a Textbook</button></a>
            <a href="sellBook.php"><button class="sell-button">Sell a Textbook</button></a>
        </div>

        <!-- Best Sellers section with book products -->
        <section class="best-sellers">
            <h2>Best Sellers</h2>
            <div class="product-grid">

                <?php
                // Check if there are results
                if ($result && mysqli_num_rows($result) > 0) {
                    while ($row = mysqli_fetch_assoc($result)) {
                        echo '<div class="product">';
                        echo '<img src="' . $row['image_path'] . '" alt="' . $row['title'] . '">';
                        echo '<h3>' . $row['title'] . '</h3>';
                        echo '<p>Author: ' . $row['author'] . '</p>';
                        echo '</div>';
                    }
                } else {
                    echo '<p>No best sellers available.</p>';
                }
                ?>

            </div>
        </section>
    </main>

    <footer>
        <!-- Add footer content if needed -->

        <!-- Full-width image at the bottom -->
        <img src="images\Bottom.png" alt="Bottom Image" class="full-width-image">

        <!-- Newsletter subscription form -->
        <div class="newsletter-form">
            <label for="email">Subscribe to our Newsletter:</label>
            <input type="email" id="email" name="email" placeholder="Your email address">
            <button type="submit">Subscribe</button>
        </div>
        <div id="admin-button">
            <button onclick="location.href='admin.php'">Admin</button>
        </div>
        <p>&copy; 2023 Your Website. All rights reserved.</p>
    </footer>

</body>

</html>

<?php
mysqli_close($conn);
?>
