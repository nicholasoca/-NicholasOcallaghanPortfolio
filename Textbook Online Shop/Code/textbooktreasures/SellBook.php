<?php
// Assuming you've established a database connection
$conn = mysqli_connect("localhost", "root", "", "bookstore");

if ($_SERVER["REQUEST_METHOD"] == "POST") {
    // Collect form data
    $bookTitle = mysqli_real_escape_string($conn, $_POST['bookTitle']);
    $authorName = mysqli_real_escape_string($conn, $_POST['authorName']);
    $price = mysqli_real_escape_string($conn, $_POST['price']);
    $condition = mysqli_real_escape_string($conn, $_POST['condition']);
    $description = mysqli_real_escape_string($conn, $_POST['description']);

    // Insert data into 'sellBook' table using prepared statements
    $insertQuery = "INSERT INTO sellBook (title, author, price, book_condition, description) VALUES (?, ?, ?, ?, ?)";
    $stmt = mysqli_prepare($conn, $insertQuery);
    mysqli_stmt_bind_param($stmt, "ssdss", $bookTitle, $authorName, $price, $condition, $description);

    if (mysqli_stmt_execute($stmt)) {
        echo "Book added successfully.";
    } else {
        echo "Error: " . $insertQuery . "<br>" . mysqli_error($conn);
    }

    // Close the prepared statement
    mysqli_stmt_close($stmt);
}

// Close the database connection
mysqli_close($conn);
?>

<!DOCTYPE html>
<html lang="en" class="sell">

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

    <!-- Main Content Section -->
    <main>
        <!-- Body content for Sell Book page -->
        <div class="image-container">
            <img src="images\Sell1.png" alt="Book for Sale">
        </div>
        <div class="sell-form">
            <h2>Sell Your Book</h2>
            <form action="sellBook.php" method="post" class="sell-form">
                <label for="bookTitle">Book Title:</label>
                <input type="text" id="bookTitle" name="bookTitle" required>
                <label for="authorName">Author Name:</label>
                <input type="text" id="authorName" name="authorName" required>

                <label for="price">Price (R):</label>
                <input type="number" id="price" name="price" required>

                <label for="condition">Condition:</label>
                <select id="condition" name="condition">
                    <option value="new">New</option>
                    <option value="like-new">Like New</option>
                    <option value="good">Good</option>
                    <option value="acceptable">Acceptable</option>
                </select>

                <label for="description">Description:</label>
                <textarea id="description" name="description" rows="4"></textarea>

                <button type="submit">Submit Listing</button>
            </form>
        </div>
    </main>

    <!-- Footer Section -->
    <footer>
        <!-- Add footer content if needed -->
		 <img src="images\Bottom.png" alt="Footer Image">
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
