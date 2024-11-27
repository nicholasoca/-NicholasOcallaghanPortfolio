<?php
// Assuming you've established a database connection
$conn = mysqli_connect("localhost", "root", "", "bookstore");

$sql = "SELECT * FROM books";
$result = mysqli_query($conn, $sql);
?>

<!DOCTYPE html>
<html lang="en" class="buy">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Buy - TextBookTreasures</title>
    <link rel="stylesheet" href="styles.css">
</head>

<body class="buy-page">
    <header>
        <img src="images/Logo.png" alt="TextBookTreasures Logo" id="logo">
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
        <!-- Body content for Buy Book page (textbooks) -->

        <?php
        // Check if there are results
        if ($result && mysqli_num_rows($result) > 0) {
            while ($row = mysqli_fetch_assoc($result)) {
                echo '<div class="product">';
                echo '<img src="' . $row['image_path'] . '" alt="' . $row['title'] . '">';
                echo '<h3>' . $row['title'] . '</h3>';
                echo '<p>Author: ' . $row['author'] . '</p>';
                echo '<p>Price: R' . $row['price'] . '</p>';
                
                // Add a form to submit the book details to the cart
                echo '<form action="cart.php" method="post">';
                echo '<input type="hidden" name="book_id" value="' . $row['id'] . '">';
                echo '<input type="hidden" name="book_title" value="' . $row['title'] . '">';
                echo '<input type="hidden" name="author" value="' . $row['author'] . '">';
                echo '<input type="hidden" name="price" value="' . $row['price'] . '">';
                echo '<button type="submit" name="add_to_cart">Add to Cart</button>';
                echo '</form>';
                
                echo '</div>';
            }
        } else {
            echo '<p>No books available for purchase.</p>';
        }
        ?>

    </main>

    <footer>
        <!-- Add footer content if needed -->
        <img src="images/Bottom.png" alt="Bottom Image" class="full-width-image" >
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

<?php
mysqli_close($conn);
?>
