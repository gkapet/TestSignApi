using Com.Ascertia.ADSS.Client.API.Signing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Internals;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestSignApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestSignController : ControllerBase
    {
        private readonly string _cerPath;
        private readonly string _inPath;

        public TestSignController(IWebHostEnvironment environment)
        {
            var hostEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
            _cerPath = Path.Combine(hostEnvironment.ContentRootPath, "StaticFiles", "gov_malta_tls.pfx");
            _inPath = Path.Combine(hostEnvironment.ContentRootPath, "StaticFiles", "Doc1.pdf");
        }

        [HttpGet]
        [Route("file")]
        public IActionResult SignExistingPdf()
        {
            PdfSigningRequest obj_signRequest = new PdfSigningRequest("Government_of_Malta", _inPath);
            obj_signRequest.SetProfileId("adss:signing:profile:001");
            obj_signRequest.SetSslClientCredentials(_cerPath, ";Hg;1l?6w/A0");
            obj_signRequest.SetSigningField("Signature1");
            obj_signRequest.SetCertificateAlias("QVSEAL");
            obj_signRequest.SetSignedBy("QVSEAL");
            obj_signRequest.SetContactInfo("info@gov.mt");
            obj_signRequest.SetSigningLocation("Republic of Malta");
            obj_signRequest.SetSignatureDictionarySize(204800);
            obj_signRequest.SetLocalHash(true);
            obj_signRequest.SetRequestMode(PdfSigningRequest.HTTP);
            obj_signRequest.SetPdfSignatureMode(PdfSigningRequest.SIGNING_MODE_DETACHED);
            PdfSigningResponse obj_signingResponse = (PdfSigningResponse)obj_signRequest.Send("https://demo-adss1.signingportal.com/adss/signing/hdsi");
            var signedPdf = new MemoryStream();
            obj_signingResponse.PublishDocument(signedPdf);

            return File(signedPdf.ToArray(), "application/pdf", "TestPdf.pdf");
        }

        [HttpGet]
        [Route("create")]
        public IActionResult CreateAndSignPdf()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var document = new Document();
            var section = document.AddSection();
            section.PageSetup = new PageSetup
            {
                TopMargin = "2cm",
                BottomMargin = "2cm",
                LeftMargin = "2cm",
                RightMargin = "2cm",
            };
            section.AddParagraph("Test text");

            var streamPdf = new MemoryStream();

            var pdfRenderer = new PdfDocumentRenderer(true)
            {
                Document = document
            };
            pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.Save(streamPdf);

            PdfSigningRequest obj_signRequest = new PdfSigningRequest("Government_of_Malta", streamPdf);
            obj_signRequest.SetProfileId("adss:signing:profile:001");
            obj_signRequest.SetSslClientCredentials(_cerPath, ";Hg;1l?6w/A0");
            obj_signRequest.SetSigningField("Signature1");
            obj_signRequest.SetCertificateAlias("QVSEAL");
            obj_signRequest.SetSignedBy("QVSEAL");
            obj_signRequest.SetContactInfo("info@gov.mt");
            obj_signRequest.SetSigningLocation("Republic of Malta");
            obj_signRequest.SetSignatureDictionarySize(204800);
            obj_signRequest.SetLocalHash(true);
            obj_signRequest.SetRequestMode(PdfSigningRequest.HTTP);
            obj_signRequest.SetPdfSignatureMode(PdfSigningRequest.SIGNING_MODE_DETACHED);
            PdfSigningResponse obj_signingResponse = (PdfSigningResponse)obj_signRequest.Send("https://demo-adss1.signingportal.com/adss/signing/hdsi");
            var signedPdf = new MemoryStream();
            obj_signingResponse.PublishDocument(streamPdf);

            return File(signedPdf.ToArray(), "application/pdf", "TestPdf.pdf");
        }
    }
}
