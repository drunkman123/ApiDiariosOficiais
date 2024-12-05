//using FluentValidation;

//namespace ApiDiariosOficiais.Validation
//{
//    public partial class FindSimilar1ToNDetectaRequestValidator : AbstractValidator<FindSimilar1ToNDetectaRequest>
//    {
//        public FindSimilar1ToNDetectaRequestValidator()
//        {
//            RuleFor(x => x.CPFPesquisador)
//                .Must(HaveValue)
//                .WithMessage("CPF deve ser preenchido.")
//                .Must(IsValidCPF)
//                .WithMessage("CPF Inválido.")
//                .Matches("^[0-9]+$")
//                .WithMessage("O CPF deve conter apenas números.")
//                .MaximumLength(11)
//                .WithMessage("Tamanho máximo permitido 11 caracteres.");
//            RuleFor(x => x.Base64Image)
//                .NotEmpty().WithMessage("Imagem é obrigatória.")
//                .Must(IsBase64StringAnImage).WithMessage("Base64 Inválido.");
//            RuleFor(x => x.Lat)
//                .NotEmpty()
//                .NotNull()
//                .WithMessage("Latitude deve ser preenchido.");
//            RuleFor(x => x.Long)
//                .NotEmpty()
//                .NotNull()
//                .WithMessage("Longitude deve ser preenchido.");
//            RuleFor(x => x.DataHora)
//                .NotEmpty().WithMessage("DataHora é obrigatória.")
//                .Must(BeWithinTenMinutes).WithMessage("DataHora deve estar dentro de 10 minutos antes ou depois do horário atual.");

//        }
//        private bool BeWithinTenMinutes(DateTime dataHora)
//        {
//            if (dataHora == null)
//                return false;

//            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
//            return dataHora >= now.AddMinutes(-10) && dataHora <= now.AddMinutes(10);
//        }
//        private bool IsValidCPF(string cpf)
//        {
//            if (string.IsNullOrWhiteSpace(cpf))
//            {
//                return false;
//            }

//            // Remove non-numeric characters from CPF
//            cpf = new string(cpf.Where(char.IsDigit).ToArray());

//            // Check if CPF has 11 digits
//            if (cpf.Length != 11)
//            {
//                return false;
//            }

//            // Check if all digits are the same (e.g., 111.111.111-11)
//            if (cpf.All(d => d == cpf[0]))
//            {
//                return false;
//            }

//            // Validate CPF using the algorithm
//            int[] numbers = cpf.Select(c => int.Parse(c.ToString())).ToArray();

//            int sum = 0;
//            for (int i = 0; i < 9; i++)
//            {
//                sum += numbers[i] * (10 - i);
//            }

//            int remainder = sum % 11;

//            int expectedDigit1 = remainder < 2 ? 0 : 11 - remainder;

//            if (numbers[9] != expectedDigit1)
//            {
//                return false;
//            }

//            sum = 0;
//            for (int i = 0; i < 10; i++)
//            {
//                sum += numbers[i] * (11 - i);
//            }

//            remainder = sum % 11;

//            int expectedDigit2 = remainder < 2 ? 0 : 11 - remainder;

//            return numbers[10] == expectedDigit2;
//        }
//        private bool IsBase64StringAnImage(string base64String)
//        {
//            if (string.IsNullOrWhiteSpace(base64String))
//            {
//                return false;
//            }

//            try
//            {
//                // Check if the string is in valid base64 format
//                byte[] imageBytes = Convert.FromBase64String(base64String);

//                // Attempt to create an Image from the byte array
//                using (MemoryStream ms = new MemoryStream(imageBytes))
//                {
//                    Image image = Image.Load(ms);

//                    // Additional checks for image properties (e.g., width, height, format) can be added here if needed
//                    return true;
//                }
//            }
//            catch (FormatException)
//            {
//                // The string is not in valid base64 format
//                return false;
//            }
//            catch (Exception)
//            {
//                // An exception occurred while attempting to convert the base64 string to an Image
//                return false;
//            }
//        }
//        private bool HaveValue(string value)
//        {
//            return !string.IsNullOrWhiteSpace(value);
//        }
//    }
//}
