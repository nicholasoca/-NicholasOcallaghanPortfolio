<?php
// Assuming you've established a database connection
$conn = mysqli_connect("localhost", "root", "", "bookstore");

// Check if a user is logged in
session_start();
if (!isset($_SESSION['user_id'])) {
    // Redirect to login page or handle as needed
    header("Location: login.php");
    exit();
}

$user_id = $_SESSION['user_id'];

// Check if a book has been added to the cart
if ($_SERVER["REQUEST_METHOD"] == "POST" && isset($_POST["add_to_cart"])) {
    $book_id = $_POST["book_id"];
    $book_title = $_POST["book_title"];
    $author = $_POST["author"];
    $price = $_POST["price"];

    // Insert the book into the cart table
    $insertQuery = "INSERT INTO cart (user_id, book_id, quantity) VALUES ('$user_id', '$book_id', 1) ON DUPLICATE KEY UPDATE quantity = quantity + 1";
    mysqli_query($conn, $insertQuery);
}

// Check if a book should be removed from the cart
if ($_SERVER["REQUEST_METHOD"] == "POST" && isset($_POST["remove_from_cart"])) {
    $book_id_to_remove = $_POST["book_id_to_remove"];

    // Remove the book from the cart table
    $deleteQuery = "DELETE FROM cart WHERE user_id = '$user_id' AND book_id = '$book_id_to_remove'";
    mysqli_query($conn, $deleteQuery);
}

// Check if the "Proceed" button is clicked
if ($_SERVER["REQUEST_METHOD"] == "POST" && isset($_POST["proceed"])) {
    // Insert items from the cart into the sales table
    $insertSaleQuery = "INSERT INTO sales (user_id, book_id, quantity) 
                       SELECT user_id, book_id, quantity FROM cart WHERE user_id = '$user_id'";
    mysqli_query($conn, $insertSaleQuery);

    // Mark the items in the cart as processed
    $updateCartQuery = "UPDATE cart SET processed = true WHERE user_id = '$user_id'";
    mysqli_query($conn, $updateCartQuery);
}

// Retrieve and display cart items
$cartQuery = "SELECT c.*, b.title, b.author, b.price, b.image_path FROM cart c JOIN books b ON c.book_id = b.id WHERE c.user_id = '$user_id' AND c.processed = false";
$result = mysqli_query($conn, $cartQuery);
?>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>TextBookTreasures - Cart</title>
    <link rel="stylesheet" href="styles.css">
</head>

<body class="cart">
    <header>
        <img src="images/Logo.png" alt="TextBookTreasures Logo" id="logo">
        <h1>TextBookTreasures</h1>
        <div class="header-buttons">
           
            <a href="cart.php"><button id="cart-btn">Cart</button></a>
            <?php
            if (isset($_SESSION['user_id'])) {
                echo '<a href="logout.php"><button id="login-register-btn">Logout</button></a>';
            } else {
                echo '<a href="login.php"><button id="login-register-btn">Login/Register</button></a>';
            }
            ?>
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
            <?php
            if (isset($_SESSION['user_id'])) {
                echo '<a href="#">Profile</a>';
            } else {
                echo '<a href="login.php">Login</a>';
            }
            ?>
        </div>
    </nav>

    <main>
        <!-- Cart content goes here -->
        <h2>Your Shopping Cart</h2>
        <?php
        // Check if there are results
        if ($result && mysqli_num_rows($result) > 0) {
            while ($row = mysqli_fetch_assoc($result)) {
                echo '<div class="cart-item">';
                echo '<img src="' . $row["image_path"] . '" alt="Book Image">';
                echo '<h3>' . $row["title"] . '</h3>';
                echo '<p>Author: ' . $row["author"] . '</p>';
                echo '<p>Price: R' . $row["price"] . '</p>';
                echo '<form action="#" method="post">';
                echo '<input type="hidden" name="book_id_to_remove" value="' . $row["book_id"] . '">';
                echo '<button type="submit" name="remove_from_cart">Remove from Cart</button>';
                echo '</form>';
                echo '</div>';
            }
            // Add Continue Shopping and Proceed buttons
            echo '<div class="cart-buttons">';
            echo '<form action="buyBook.php" method="get">';
            echo '<button type="submit">Continue Shopping</button>';
            echo '</form>';
            echo '<form action="#" method="post">';
            echo '<button type="submit" name="proceed">Proceed</button>';
            echo '</form>';
            echo '</div>';
        } else {
            echo '<p>Your cart is empty.</p>';
        }

        // Free result set
        mysqli_free_result($result);

        // Close connection
        mysqli_close($conn);
        ?>
    </main>

    <footer>
        <!-- Footer content if needed -->
        <img src="images/Bottom.png" alt="Bottom Image" class="full-width-image">
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
