using Common.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Models;
using Presentation.Utilities;

namespace Presentation.Controllers
{
    
    public class CvController : Controller
    {
        private readonly CvRepository _cvRepository;
        private readonly EncryptionKeyRepository _keyRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<CvController> _logger;

        //Controller
        public CvController(
            CvRepository cvRepository,
            EncryptionKeyRepository keyRepository,
            UserManager<IdentityUser> userManager, 
            IWebHostEnvironment webHostEnvironment,
            ILogger<CvController> logger)
        {
            _cvRepository = cvRepository;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _keyRepository = keyRepository;
            _logger = logger;
        }

        [HttpGet] //Fetch page for users submitted cvs
        [Authorize]
        public async Task<IActionResult> UserCvs()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogError("User is null");
                TempData["ErrorMessage"] = "An error occurd while fetching cvs";
                return RedirectToAction("Index", "Home");
            }

            var cvs = _cvRepository.GetCvsByUserId(user.Id);
            var cvViewModels = new List<CvViewModel>();

            try
            {
                foreach (var cv in cvs)
                {
                    var userEmail = await _userManager.FindByIdAsync(cv.UserId);
                    var employerEmail = await _userManager.FindByIdAsync(cv.EmployerId);

                    var cvViewModel = new CvViewModel
                    {
                        Id = cv.Id,
                        //Add decryption here to show proper file name
                        FileName = cv.FileName.Substring(37),
                        UserEmail = userEmail.Email,
                        EmployerEmail = employerEmail.Email
                    };
                    _logger.LogInformation("Succesfully added CV's into a list");
                    cvViewModels.Add(cvViewModel);
                }
                _logger.LogInformation("The UserCvs page was succesfully loaded.");
                return View(cvViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An issue was encountered when fetching cvs");
                TempData["ErrorMessage"] = "An error occurd while fetching cvs";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet] //Fetch page for users submitted cvs for a pecific job posting
        [Authorize(Roles = "Admin,Manager,Employer")]
        public async Task<IActionResult> EmployerCvs()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogError("User is null");
                TempData["ErrorMessage"] = "An error occurd.";
                return RedirectToAction("Index", "Home");
            }

            var cvs = _cvRepository.GetCvsByEmployerId(user.Id);
            var cvViewModels = new List<CvViewModel>();
            
            try
            {
                foreach (var cv in cvs)
                {
                    var userEmail = await _userManager.FindByIdAsync(cv.UserId);
                    var employerEmail = await _userManager.FindByIdAsync(cv.EmployerId);

                    var cvViewModel = new CvViewModel
                    {
                        Id = cv.Id,
                        FileName = cv.FileName.Substring(37),
                        UserEmail = userEmail.Email,
                        EmployerEmail = employerEmail.Email
                    };

                    cvViewModels.Add(cvViewModel);
                }
                _logger.LogInformation("Page of employer Cvs was succesfully loaded");
                return View(cvViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error encountered while creating a list of cvs for the employer");
                TempData["ErrorMessage"] = "An error occurd while fetching cvs";
                return RedirectToAction("Index", "Home");
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFile cvFile, string employerId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("User attempted to submit a file with an incorrect file extension");
                TempData["ErrorMessage"] = "Only PDF and DOCX files are allowed.";
                return RedirectToAction("Index", "Jobs");
            }

            if (!IsValidFileType(cvFile))
            {
                _logger.LogError("User attempted to submit a file which didn't add up with the listed magic numbers of pdf or docx files");
                TempData["ErrorMessage"] = "Only PDF and DOCX files are allowed.";
                return RedirectToAction("Index", "Jobs");
            }

            if (cvFile == null || cvFile.Length <= 0)
            {
                _logger.LogError("User attempted to upload an empty file or no file");
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return RedirectToAction("Index", "Jobs");
            }

            if (cvFile.Length > 10 * 1024 * 1024)
            {
                _logger.LogError($"User attempted to upload a file larger than 10MB, {cvFile.Length}");
                TempData["ErrorMessage"] = "File size cannot be greater than 10MB.";
                return RedirectToAction("Index", "Jobs");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("User was not logged in.");
                TempData["ErrorMessage"] = "An error has occurred";
                return RedirectToAction("Index", "Home");
            }

            var employerKey = _keyRepository.GetKeyById(employerId);
            if (employerKey.PublicKey == null)
            {
                _logger.LogError("Could not fetch the employers public key");
                TempData["ErrorMessage"] = "Employer's encryption key not found.";
                return RedirectToAction("Index", "Jobs");
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Data");
            if (!Directory.Exists(uploadsFolder))
            {
                _logger.LogInformation($"{uploadsFolder} directory was created");
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + cvFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    await cvFile.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                var e = new Encryption();

                // Encrypt the file using hybrid encryption using the employers public key
                MemoryStream encryptedStream = e.HybridEncrypt(fileBytes, employerKey.PublicKey);

                var userPrivateKey = _keyRepository.GetKeyById(user.Id);
                if (userPrivateKey.PrivateKey == null)
                {
                    _logger.LogInformation($"{userPrivateKey} does not exist");
                    TempData["ErrorMessage"] = "User's encryption key not found.";
                    return RedirectToAction("Index", "Jobs");
                }

                // Create digital signature using the users private key
                byte[] signature = e.DigitalSign(fileBytes, userPrivateKey.PrivateKey);

                CV cv = new CV()
                {
                    FileName = uniqueFileName,
                    UserId = user.Id,
                    EmployerId = employerId,
                    DigtalSignature = signature
                };

                // Save CV
                _logger.LogInformation("New cv uploaded");
                _cvRepository.AddCv(cv);

                // Save the encrypted file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    _logger.LogInformation("File saved");
                    encryptedStream.Position = 0;
                    await encryptedStream.CopyToAsync(fileStream);
                }

                TempData["SuccessMessage"] = "CV uploaded successfully!";
                return RedirectToAction("Index", "Jobs");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while saving the file or saving the cv to the database");
                TempData["ErrorMessage"] = "An error occurred while uploading the file";
                return RedirectToAction("Index", "Jobs");
            }
        }

        private bool IsValidFileType(IFormFile cvFile)
        {
            byte[] buffer = new byte[8];
            using (var stream = cvFile.OpenReadStream())
            {
                stream.Read(buffer, 0, buffer.Length);
            }

            int[] whiteListPdf = new int[] { 37, 80, 68, 70 };
            int[] whiteListDocx = new int[] { 80, 75, 3, 4, 20, 0, 6, 0 };

            //Start off by checking if its a PDF
            if (buffer.Length >= 4)
            {
                bool isPdf = true;
                bool isDocx = true;

                //Go through the list of bytes to check if they match up
                for (int i = 0; i < 4; i++)
                {
                    if (buffer[i] != whiteListPdf[i])
                    {
                        isPdf = false;
                    }
                    if (buffer[i] != whiteListDocx[i])
                    {
                        isDocx = false;
                    }
                }

                if (buffer.Length >= 8 && isDocx) 
                {
                    for (int i = 4; i < 8; i++)
                    {
                        if (buffer[i] != whiteListDocx[i])
                        {
                            isDocx = false;
                            break;
                        }
                    }
                }
                return isPdf || isDocx;
            }
            return false;
        }// Close File type Validation

        [Authorize(Roles = "Admin,Manager,Employer")]
        public async Task<IActionResult> Download(int cvId)
        {
            var cv = _cvRepository.GetAllCv().SingleOrDefault(x => x.Id == cvId);
            if (cv == null)
            {
                TempData["ErrorMessage"] = "An error occurred while downloading the file";
                return RedirectToAction("EmployerCvs", "Cv");
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Data");
            var pathToCv = uploadsFolder + "\\" + cv.FileName;

            if (!System.IO.File.Exists(pathToCv))
            {
                _logger.LogError($"File {pathToCv}, does not exist");
                TempData["ErrorMessage"] = "File does not exist.";
                return RedirectToAction("EmployerCvs", "Cv");
            }

            try
            {
                byte[] encryptedFileBytes = System.IO.File.ReadAllBytes(pathToCv);

                var e = new Encryption();

                var employer = await _userManager.FindByIdAsync(cv.EmployerId);
                var employerKey = _keyRepository.GetKeyById(employer.Id);
                if (employerKey.PrivateKey == null)
                {
                    _logger.LogError($"{employer.Id}, does not have a private key");
                    TempData["ErrorMessage"] = "Employer's encryption key not found.";
                    return RedirectToAction("EmployerCvs", "Cv");
                }

                // Decrypt the file
                MemoryStream decryptedStream = e.HybridDecrypt(encryptedFileBytes, employerKey.PrivateKey);
                byte[] decryptedFileBytes = decryptedStream.ToArray();

                var userKey = _keyRepository.GetKeyById(cv.UserId);
                if (userKey.PublicKey == null)
                {
                    _logger.LogError($"{cv.UserId}, does not have a private key");
                    TempData["ErrorMessage"] = "User's encryption key not found.";
                    return RedirectToAction("EmployerCvs", "Cv");
                }

                // Verify the digital signature with the original file bytes (decrypted)
                bool verifySignature = e.DigitalVerification(decryptedFileBytes, cv.DigtalSignature, userKey.PublicKey);
                if (!verifySignature)
                {
                    _logger.LogError($"{cv.DigtalSignature}, could not be verified");
                    TempData["ErrorMessage"] = "File signature could not be verified.";
                    return RedirectToAction("EmployerCvs", "Cv");
                }

                string fileName = cv.FileName.Substring(37);

                return File(decryptedFileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}, error occured while downloading file");
                TempData["ErrorMessage"] = "An error occurred while downloading the file.";
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
