<?php
$conn = mysqli_connect("localhost", "root", "", "bookstore");

// Function to approve a sale
function approveSale($saleId) {
    global $conn;
    // Implement your logic to update the sale status in the database
    $updateQuery = "UPDATE sales SET status = 'approved' WHERE id = $saleId";
    mysqli_query($conn, $updateQuery);
}

// Function to decline a sale
function declineSale($saleId) {
    global $conn;
    // Implement your logic to update the sale status in the database
    $updateQuery = "UPDATE sales SET status = 'declined' WHERE id = $saleId";
    mysqli_query($conn, $updateQuery);
}

// Function to get all sales grouped by user with book information
function getAllSalesGroupedByUserWithBookInfo() {
    global $conn;
    // Implement your logic to retrieve all sales from the database grouped by user with book information
    $selectQuery = "SELECT s.id, s.user_id, s.book_id, s.quantity, s.status, u.email AS customer_email, b.title AS book_title
                    FROM sales s
                    JOIN users u ON s.user_id = u.id
                    JOIN books b ON s.book_id = b.id
                    ORDER BY s.user_id, s.id";
    $result = mysqli_query($conn, $selectQuery);

    // Check if the query was successful
    if (!$result) {
        die("Error retrieving sales: " . mysqli_error($conn));
    }

    return $result;
}

// Function to add a new book
function addBook($title, $author, $price, $imagePath) {
    global $conn;
    // Implement your logic to insert a new book into the database
    $insertQuery = "INSERT INTO books (title, author, price, image_path) VALUES ('$title', '$author', $price, '$imagePath')";
    mysqli_query($conn, $insertQuery);
}

// Function to delete a book
function deleteBook($bookId) {
    global $conn;
    // Implement your logic to delete a book from the database
    $deleteQuery = "DELETE FROM books WHERE id = $bookId";
    mysqli_query($conn, $deleteQuery);
}

// Function to update book details
function updateBook($bookId, $title, $author, $price, $imagePath) {
    global $conn;
    // Implement your logic to update book details in the database
    $updateQuery = "UPDATE books SET title = '$title', author = '$author', price = $price, image_path = '$imagePath' WHERE id = $bookId";
    mysqli_query($conn, $updateQuery);
}

// Function to add a new user
function addUser($email, $password) {
    global $conn;
    // Implement your logic to insert a new user into the database
    // Note: Always use secure password hashing methods
    $hashedPassword = password_hash($password, PASSWORD_DEFAULT);
    $insertQuery = "INSERT INTO users (email, password) VALUES ('$email', '$hashedPassword')";
    mysqli_query($conn, $insertQuery);
}

// Function to delete a user
function deleteUser($userId) {
    global $conn;
    // Implement your logic to delete a user from the database
    $deleteQuery = "DELETE FROM users WHERE id = $userId";
    mysqli_query($conn, $deleteQuery);
}

// Function to update user details
function updateUser($userId, $email, $password) {
    global $conn;
    // Implement your logic to update user details in the database
    // Note: Always use secure password hashing methods
    $hashedPassword = password_hash($password, PASSWORD_DEFAULT);
    $updateQuery = "UPDATE users SET email = '$email', password = '$hashedPassword' WHERE id = $userId";
    mysqli_query($conn, $updateQuery);
}

// Check for form submissions and call corresponding functions
if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    if (isset($_POST['add_book'])) {
        addBook($_POST['title'], $_POST['author'], $_POST['price'], $_POST['imagePath']);
    } elseif (isset($_POST['delete_book'])) {
        deleteBook($_POST['deleteBookId']);
    } elseif (isset($_POST['update_book'])) {
        updateBook($_POST['updateBookId'], $_POST['updateTitle'], $_POST['updateAuthor'], $_POST['updatePrice'], $_POST['updateImagePath']);
    } elseif (isset($_POST['add_user'])) {
        addUser($_POST['userEmail'], $_POST['userPassword']);
    } elseif (isset($_POST['delete_user'])) {
        deleteUser($_POST['deleteUserId']);
    } elseif (isset($_POST['update_user'])) {
        updateUser($_POST['updateUserId'], $_POST['updateUserEmail'], $_POST['updateUserPassword']);
    }
}
?>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="stylesheet" href="styles.css">
	
	<style>
        body {
            font-family: 'Arial', sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }

        header {
            background-color: #333;
            color: #fff;
            text-align: center;
            padding: 1em 0;
        }

        main {
            max-width: 800px;
            margin: 20px auto;
            background-color: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }

        h2 {
            color: #333;
            border-bottom: 2px solid #333;
            padding-bottom: 8px;
            margin-top: 20px;
        }

        .user-sales {
            border: 1px solid #ddd;
            border-radius: 8px;
            padding: 10px;
            margin-bottom: 20px;
            background-color: #fff;
        }

        .sale-item {
            border: 1px solid #ddd;
            border-radius: 8px;
            padding: 10px;
            margin-bottom: 20px;
            background-color: #fff;
        }

        .sale-item p {
            margin: 5px 0;
        }

        button {
            background-color: #4caf50;
            color: #fff;
            border: none;
            padding: 10px 20px;
            text-align: center;
            text-decoration: none;
            display: inline-block;
            font-size: 16px;
            margin: 4px 2px;
            cursor: pointer;
            border-radius: 4px;
            transition: background-color 0.3s ease;
        }

        button.decline {
            background-color: #f44336;
        }

        button:hover {
            background-color: #45a049;
        }

        form {
            margin-bottom: 20px;
        }

        label {
            display: block;
            margin-bottom: 8px;
        }

        input {
            width: 100%;
            padding: 8px;
            margin-bottom: 12px;
            box-sizing: border-box;
            border: 1px solid #ccc;
            border-radius: 4px;
        }

        footer {
            background-color: #333;
            color: #fff;
            padding: 10px 0;
            text-align: center;
        }

        .newsletter-form {
            display: flex;
            justify-content: center;
            align-items: center;
            margin-bottom: 10px;
        }

        .newsletter-form input {
            margin-right: 10px;
        }
    </style>
	
    <title>Admin Page</title>
</head>

<body>

    <header>
        <div id="web-title">
            <h1>Admin Page</h1>
        </div>
    </header>

      <main>
    <!-- Admin content goes here -->
	<h2>All Sales</h2>
		<?php
		$sales = getAllSalesGroupedByUserWithBookInfo();

		if (empty($sales)) {
        echo "<p>No sales to display.</p>";
		} else {
        $currentUser = null;

        foreach ($sales as $sale) {
            // Check if user has changed
            if ($currentUser !== $sale['user_id']) {
                // If it's not the first user, close the previous section
                if ($currentUser !== null) {
                    echo '</div>';
                }

                // Start a new section for the current user
                echo '<div class="user-sales">';
                echo '<h3>User ID: ' . $sale['user_id'] . ' - ' . $sale['customer_email'] . '</h3>';
                $currentUser = $sale['user_id'];
            }

            echo '<div class="sale-item">';
            echo '<p>Sale ID: ' . $sale['id'] . '</p>';
            echo '<p>Book Name: ' . $sale['book_title'] . '</p>'; // Display Book Name
            echo '<p>Book ID: ' . $sale['book_id'] . '</p>';
            echo '<p>Quantity: ' . $sale['quantity'] . '</p>';
            echo '<p>Status: ' . $sale['status'] . '</p>';

            // Check if the sale is pending
            if ($sale['status'] == 'pending') {
                echo '<form action="#" method="post">';
                echo '<input type="hidden" name="sale_id" value="' . $sale['id'] . '">';

                // Approve Sale Button
                echo '<button type="submit" name="approve_sale">Approve Sale</button>';

                // Decline Sale Button
                echo '<button type="submit" name="decline_sale" class="decline">Decline Sale</button>';

                echo '</form>';
            }

            echo '</div>';
        }

        // Close the last section
        echo '</div>';
    }
	?>
	<!-- Add Book Form -->
<h2>Add Book</h2>
<form action="#" method="post">
    <label for="title">Title:</label>
    <input type="text" id="title" name="title" required>
    <label for="author">Author:</label>
    <input type="text" id="author" name="author" required>
    <label for="price">Price:</label>
    <input type="text" id="price" name="price" required>
    <label for="imagePath">Image Path:</label>
    <input type="text" id="imagePath" name="imagePath" required>
    <button type="submit" name="add_book">Add Book</button>
</form>

<!-- Delete Book Form -->
<h2>Delete Book</h2>
<form action="#" method="post">
    <label for="deleteBookId">Book ID:</label>
    <input type="text" id="deleteBookId" name="deleteBookId" required>
    <button type="submit" name="delete_book">Delete Book</button>
</form>

<!-- Update Book Form -->
<h2>Update Book</h2>
<form action="#" method="post">
    <label for="updateBookId">Book ID:</label>
    <input type="text" id="updateBookId" name="updateBookId" required>
    <label for="updateTitle">Title:</label>
    <input type="text" id="updateTitle" name="updateTitle" required>
    <label for="updateAuthor">Author:</label>
    <input type="text" id="updateAuthor" name="updateAuthor" required>
    <label for="updatePrice">Price:</label>
    <input type="text" id="updatePrice" name="updatePrice" required>
    <label for="updateImagePath">Image Path:</label>
    <input type="text" id="updateImagePath" name="updateImagePath" required>
    <button type="submit" name="update_book">Update Book</button>
</form>

<!-- Add User Form -->
<h2>Add User</h2>
<form action="#" method="post">
    <label for="userEmail">Email:</label>
    <input type="email" id="userEmail" name="userEmail" required>
    <label for="userPassword">Password:</label>
    <input type="password" id="userPassword" name="userPassword" required>
    <button type="submit" name="add_user">Add User</button>
</form>

<!-- Delete User Form -->
<h2>Delete User</h2>
<form action="#" method="post">
    <label for="deleteUserId">User ID:</label>
    <input type="text" id="deleteUserId" name="deleteUserId" required>
    <button type="submit" name="delete_user">Delete User</button>
</form>

<!-- Update User Form -->
<h2>Update User</h2>
<form action="#" method="post">
    <label for="updateUserId">User ID:</label>
    <input type="text" id="updateUserId" name="updateUserId" required>
    <label for="updateUserEmail">Email:</label>
    <input type="email" id="updateUserEmail" name="updateUserEmail" required>
    <label for="updateUserPassword">Password:</label>
    <input type="password" id="updateUserPassword" name="updateUserPassword" required>
    <button type="submit" name="update_user">Update User</button>
</form>
</main>

    

</body>

</html>
