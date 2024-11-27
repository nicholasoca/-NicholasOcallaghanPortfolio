<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="captureModule.aspx.cs" Inherits="User_Registration.captureModule" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Capture Module</title>
    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f8f9fa;
            margin: 0;
            padding: 0;
        }

        .container {
            max-width: 800px;
            margin: 50px auto;
            padding: 20px;
            background-color: #fff;
            border: 1px solid #ddd;
            border-radius: 5px;
        }

        h2, h3 {
            color: #007bff;
        }

        label {
            font-weight: bold;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .btn-primary {
            background-color: #007bff;
            color: #fff;
            border: none;
            padding: 10px 20px;
            cursor: pointer;
        }

        .table {
            width: 100%;
            margin-top: 20px;
            border-collapse: collapse;
            border: 1px solid #ddd;
        }

        .table th, .table td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
        }

        .table th {
            background-color: #007bff;
            color: #fff;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Module Details</h2>

            <!-- Module Information -->
            <div class="form-group">
                <label for="moduleCode">Module Code:</label>
                <asp:TextBox runat="server" ID="moduleCode" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label for="moduleName">Module Name:</label>
                <asp:TextBox runat="server" ID="moduleName" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label for="moduleCredit">Module Credit:</label>
                <asp:TextBox runat="server" ID="moduleCredit" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label for="classHoursPerWeek">Class Hours Per Week:</label>
                <asp:TextBox runat="server" ID="classHoursPerWeek" CssClass="form-control" />
            </div>

            <!-- Semester Information -->
            <h3>Semester Information</h3>

            <div class="form-group">
                <label for="numberOfWeeks">Number of Weeks:</label>
                <asp:TextBox runat="server" ID="numberOfWeeks" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label for="startDate">Start Date:</label>
                <asp:TextBox runat="server" ID="startDate" CssClass="form-control" TextMode="Date" />
            </div>

            <!-- Record Hours -->
            <h3>Record Hours</h3>

            <div class="form-group">
                <label for="numberOfHoursWorked">Number of Hours Worked:</label>
                <asp:TextBox runat="server" ID="numberOfHoursWorked" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label for="dateWorked">Date Worked:</label>
                <asp:TextBox runat="server" ID="dateWorked" CssClass="form-control" TextMode="Date" />
            </div>
            <asp:Label ID="lblSuccessMessage" runat="server" ForeColor="Green" />
            <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" />
            <asp:Label ID="Label1" runat="server" ForeColor="Red" />


            <!-- List view code -->
            <table class="table">
                <thead>
                    <tr>
                        <th>Module Code</th>
                        <th>Module Name</th>
                        <th>Module Credit</th>
                        <th>Class Hours Per Week</th>
                        <th>Number of Weeks</th>
                        <th>Start Date</th>
                        <th>Number of Hours Worked</th>
                        <th>Date Worked</th>
                    </tr>
                </thead>
                <tbody id="lvModules" runat="server">
                    <!-- Data will be dynamically added here using JavaScript -->
                </tbody>
            </table>

            <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" OnClientClick="addToListView(); return false;" Text="Submit" />
            <asp:Button runat="server" ID="btnSaveToDatabase" CssClass="btn btn-primary" OnClick="btnSaveToDatabase_Click" Text="Save to Database" />

        </div>

         <script>
             // Updated JavaScript code
             function addToListView() {
                 var moduleCode = document.getElementById('<%= moduleCode.ClientID %>').value;
                var moduleName = document.getElementById('<%= moduleName.ClientID %>').value;
                var moduleCredit = document.getElementById('<%= moduleCredit.ClientID %>').value;
                var classHoursPerWeek = document.getElementById('<%= classHoursPerWeek.ClientID %>').value;
                var numberOfWeeks = document.getElementById('<%= numberOfWeeks.ClientID %>').value;
                var startDate = document.getElementById('<%= startDate.ClientID %>').value;
                var numberOfHoursWorked = document.getElementById('<%= numberOfHoursWorked.ClientID %>').value;
                var dateWorked = document.getElementById('<%= dateWorked.ClientID %>').value;

                if (!validateClientInput(moduleCredit, classHoursPerWeek, numberOfWeeks, startDate, numberOfHoursWorked, dateWorked)) {
                    return false;
                }

                var newRow = document.createElement('tr');
                newRow.innerHTML = `
                    <td>${moduleCode}</td>
                    <td>${moduleName}</td>
                    <td>${moduleCredit}</td>
                    <td>${classHoursPerWeek}</td>
                    <td>${numberOfWeeks}</td>
                    <td>${startDate}</td>
                    <td>${numberOfHoursWorked}</td>
                    <td>${dateWorked}</td>
                `;

                document.getElementById('<%= lvModules.ClientID %>').appendChild(newRow);

                clearInputFields();
                saveToDatabase();
            }

            function validateClientInput(moduleCredit, classHoursPerWeek, numberOfWeeks, startDate, numberOfHoursWorked, dateWorked) {
                // Validation logic remains unchanged
            }

            function clearInputFields() {
                document.getElementById('<%= moduleCode.ClientID %>').value = '';
                document.getElementById('<%= moduleName.ClientID %>').value = '';
                document.getElementById('<%= moduleCredit.ClientID %>').value = '';
                document.getElementById('<%= classHoursPerWeek.ClientID %>').value = '';
                document.getElementById('<%= numberOfWeeks.ClientID %>').value = '';
                document.getElementById('<%= startDate.ClientID %>').value = '';
                document.getElementById('<%= numberOfHoursWorked.ClientID %>').value = '';
                document.getElementById('<%= dateWorked.ClientID %>').value = '';
            }

            function saveToDatabase() {
                var data = [];
                var tableRows = document.getElementById('<%= lvModules.ClientID %>').rows;

                 for (var i = 0; i < tableRows.length; i++) {
                     var row = tableRows[i];
                     data.push({
                         moduleCode: row.cells[0].innerText,
                         moduleName: row.cells[1].innerText,
                         moduleCredit: row.cells[2].innerText,
                         classHoursPerWeek: row.cells[3].innerText,
                         numberOfWeeks: row.cells[4].innerText,
                         startDate: row.cells[5].innerText,
                         numberOfHoursWorked: row.cells[6].innerText,
                         dateWorked: row.cells[7].innerText
                     });
                 }

                 $.ajax({
                     type: "POST",
                     url: "captureModule.aspx/SaveData",
                     data: JSON.stringify({ modules: data }),
                     contentType: "application/json; charset=utf-8",
                     dataType: "json",
                     success: function (response) {
                         console.log(response.d);
                         // Handle success response
                     },
                     error: function (error) {
                         console.log(error);
                         // Handle error
                     }
                 });
             }
         </script>
    </form>
</body>
</html>
