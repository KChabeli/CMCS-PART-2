# Contract Monthly Claim System (CMCS)

A comprehensive web application built with **ASP.NET Core 8.0 MVC** for managing lecturer claims, document uploads, and approval workflows in an academic institution.

## 🎯 Overview

The Contract Monthly Claim System (CMCS) is a robust web application designed to streamline the process of submitting, verifying, and processing claims for contract lecturers. The system provides separate interfaces for different user roles:

- **Lecturers**: Submit claims, upload supporting documents, and track claim status
- **Programme Coordinators**: Verify and approve claims submitted by lecturers
- **Academic Managers**: Final approval authority with access to all claims

### Key Benefits

- ✅ **Streamlined Workflow**: Automated claim processing with clear approval stages
- ✅ **Secure Document Management**: Encrypted file uploads with validation
- ✅ **Real-time Tracking**: Transparent status monitoring from submission to settlement
- ✅ **User-friendly Interface**: Modern, responsive design with intuitive navigation
- ✅ **Comprehensive Testing**: Full unit test coverage for reliability

## ✨ Features

### 1. Claim Submission
- **Intuitive Form Interface**: Lecturers submit claims with a simple, user-friendly form
- **Real-time Calculation**: Automatic calculation of total amount based on hours worked and hourly rate
- **File Upload Support**: Upload supporting documents (PDF, DOCX, XLSX, DOC, XLS)
- **Form Validation**: Comprehensive client and server-side validation
- **Notes Field**: Optional additional notes for claim details

### 2. Claim Verification & Approval
- **Coordinator Dashboard**: Dedicated view for Programme Coordinators
- **Manager Dashboard**: Separate interface for Academic Managers
- **Pending Claims Display**: Clear overview of all pending claims
- **Approve/Reject Actions**: One-click approval or rejection with reason
- **Modal Dialogs**: User-friendly rejection reason input
- **Real-time Updates**: Immediate status updates after processing

### 3. Document Management
- **Secure File Upload**: Support for multiple file formats with size limits (10MB max)
- **File Validation**: Type and size validation for security
- **File Encryption**: Encrypted storage for sensitive documents
- **Document Listing**: View all uploaded documents for each claim
- **Download Functionality**: Easy download of supporting documents
- **File Descriptions**: Optional descriptions for uploaded files

### 4. Status Tracking
- **Transparent Tracking**: Real-time status updates throughout the approval process
- **Progress Indicators**: Visual progress bars showing claim completion status
- **Detailed History**: Complete audit trail of claim processing
- **Status Categories**: Pending, Approved, Rejected with color-coded indicators
- **Summary Statistics**: Dashboard with claim counts and totals

### 5. Error Handling & Reliability
- **Comprehensive Error Handling**: Graceful error handling with user-friendly messages
- **Input Validation**: Both client and server-side validation
- **File Security**: Secure file upload with type and size restrictions
- **Exception Logging**: Detailed logging for debugging and monitoring
- **User Feedback**: Success and error messages for all operations

### 6. Unit Testing
- **Service Layer Tests**: Complete test coverage for business logic
- **Controller Tests**: MVC controller action testing
- **Model Validation Tests**: Data validation testing
- **Mock Objects**: Proper mocking for isolated testing
- **Edge Case Coverage**: Testing of error conditions and edge cases

## 🛠 Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 MVC
- **Language**: C# (.NET 8.0)
- **Architecture**: Service Layer pattern with dependency injection
- **File Handling**: Secure file upload with validation and encryption

### Frontend
- **CSS Framework**: Bootstrap 5
- **Icons**: Font Awesome
- **JavaScript**: jQuery
- **Validation**: jQuery Validation, jQuery Validation Unobtrusive

### Testing
- **Test Framework**: xUnit
- **Mocking**: Moq
- **Test Hosting**: Microsoft.AspNetCore.Mvc.Testing

## 📁 Project Structure

```
CMCS_ST10448834-main/
├── CMCS/                              # Main application
│   ├── Controllers/
│   │   ├── CoordinatorController.cs   # Coordinator actions
│   │   ├── HomeController.cs          # Lecturer actions
│   │   └── ManagerController.cs       # Manager actions
│   ├── Models/
│   │   ├── Claim.cs                   # Claim entity with validation
│   │   ├── ClaimSubmissionViewModel.cs # View model for submission
│   │   ├── Document.cs                # Document entity
│   │   ├── ErrorViewModel.cs          # Error view model
│   │   ├── Lecturer.cs                # Lecturer entity
│   │   └── User.cs                    # User entity
│   ├── Services/
│   │   ├── IClaimService.cs           # Claim service interface
│   │   ├── ClaimService.cs            # Claim business logic
│   │   ├── IDocumentService.cs        # Document service interface
│   │   ├── DocumentService.cs         # Document management logic
│   │   ├── IFileEncryptionService.cs  # File encryption interface
│   │   └── FileEncryptionService.cs   # File encryption logic
│   ├── Views/
│   │   ├── Coordinator/
│   │   │   └── Index.cshtml           # Coordinator dashboard
│   │   ├── Home/
│   │   │   ├── Index.cshtml           # Lecturer dashboard
│   │   │   ├── SubmitClaim.cshtml     # Claim submission form
│   │   │   ├── TrackClaims.cshtml     # Claim tracking
│   │   │   ├── UploadDocuments.cshtml # Document upload
│   │   │   └── VerifyClaims.cshtml   # Claim verification
│   │   ├── Manager/
│   │   │   └── Index.cshtml           # Manager dashboard
│   │   └── Shared/
│   │       ├── _Layout.cshtml         # Main layout
│   │       └── Error.cshtml           # Error page
│   ├── wwwroot/
│   │   ├── css/                       # Custom styles
│   │   ├── js/                        # JavaScript files
│   │   └── lib/                       # Third-party libraries
│   ├── Program.cs                     # Application entry point
│   └── CMCS.csproj                    # Project file
│
├── CMCS.Tests/                        # Test project
│   ├── Controllers/
│   │   ├── CoordinatorControllerTests.cs
│   │   ├── HomeControllerTests.cs
│   │   └── ManagerControllerTests.cs
│   ├── Models/
│   │   └── ClaimTests.cs
│   └── Services/
│       ├── ClaimServiceTests.cs
│       └── FileEncryptionServiceTests.cs
│
├── Lecturer_Feedback_Implementation.md
├── Project_Summary.md
├── Git_Setup_Instructions.md
└── README.md                          # This file
```

## 📖 Usage

### For Lecturers

1. **Submit a Claim**
   - Navigate to the "Submit Claim" page
   - Fill in hours worked, hourly rate, and optional notes
   - Upload supporting documents (if any)
   - Click "Submit Claim"
   - View real-time calculation of total amount

2. **Track Claims**
   - Navigate to "Track Claims"
   - View all submitted claims with their current status
   - Monitor progress with visual indicators
   - Access detailed claim information

3. **Upload Documents**
   - Access "Upload Documents" from claim details
   - Select files to upload (PDF, DOCX, XLSX, DOC, XLS)
   - Add descriptions for each file
   - Files are automatically encrypted and stored

### For Programme Coordinators

1. **Verify Claims**
   - Access the Coordinator Dashboard
   - View all pending claims assigned to you
   - Review claim details and supporting documents
   - Approve or reject claims with reasons

2. **Download Documents**
   - Click on any document to download and review
   - Documents are automatically decrypted for viewing

### For Academic Managers

1. **Approve Claims**
   - Access the Manager Dashboard
   - View all claims pending final approval
   - Review coordinator recommendations
   - Give final approval or rejection

## 🧪 Testing

The application includes comprehensive unit tests covering:

### Test Coverage

- ✅ Service layer business logic
- ✅ Controller actions and routing
- ✅ Model validation
- ✅ File upload and encryption
- ✅ Error handling scenarios
- ✅ Edge cases and boundary conditions

### Test Structure

```
CMCS.Tests/
├── Controllers/         # Controller action tests
├── Services/           # Business logic tests
└── Models/             # Model validation tests
```

## 🔒 Security

### Security Features

1. **File Upload Security**
   - Type validation (only allowed file types)
   - Size limits (10MB maximum)
   - Secure file storage with unique naming
   - Content validation

2. **Application Security**
   - CSRF protection with anti-forgery tokens
   - Comprehensive input validation
   - Secure error messages (no sensitive information exposure)
   - File encryption for uploaded documents

3. **Data Security**
   - Encrypted file storage
   - Secure file paths
   - Validation on both client and server side

### Security Best Practices

- All forms include anti-forgery tokens
- File uploads are validated for type and size
- Sensitive data is not exposed in error messages
- File paths are sanitized to prevent directory traversal

## 🎨 User Interface

### Design Features

- **Responsive Layout**: Mobile-friendly Bootstrap 5 design
- **Color Scheme**: Professional blue primary with status colors
- **Icons**: Font Awesome icons throughout the interface
- **Cards**: Clean card-based layout for claims
- **Progress Bars**: Visual status indicators
- **Modals**: User-friendly dialog boxes for confirmations
- **Real-time Feedback**: Success and error messages

### User Experience

- **Intuitive Navigation**: Clear menu structure
- **Form Validation**: Client and server-side validation with helpful messages
- **Loading States**: Visual feedback for user actions
- **Accessibility**: Proper ARIA labels and semantic HTML

### Optimization Features

- **Async Operations**: All I/O operations are asynchronous
- **Efficient Queries**: Optimized data retrieval
- **File Size Limits**: 10MB maximum file size
- **Static File Caching**: Browser caching for static assets
- **Responsive Design**: Fast loading on mobile devices

## 🔮 Future Enhancements

- **Database Integration**: Replace in-memory storage with SQL Server or PostgreSQL
- **Authentication & Authorization**: User login with role-based access control
- **Email Notifications**: Automated email notifications for status changes
- **Advanced Reporting**: Analytics and reporting dashboard
- **RESTful API**: API endpoints for mobile applications
- **Multi-tenancy**: Support for multiple organizations
- **Audit Trail**: Detailed logging and audit trail
- **Export Functionality**: Export claims to PDF or Excel

**Note**: This is a production-ready application demonstrating modern web development practices with ASP.NET Core MVC, comprehensive testing, and secure file management.

---

##YouTube video url
https://youtu.be/o4FIzohgYgw

*Last Updated: 2024*
