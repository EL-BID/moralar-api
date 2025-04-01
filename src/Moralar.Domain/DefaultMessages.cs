using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace Moralar.Domain
{
    public class DefaultMessages
    {

        /*REQUIRED*/
        public const string FieldRequired = "Informe o campo {0}";
        public const string Minlength = "Informe no mínimo {1} caracteres no campo {0}.";
        public const string Maxlength = "Informe no máximo {1} caracteres no campo {0}.";
        public const string Range = "Informe no mínimo {1} e no maxímo {2} caracteres no campo {0}.";
        public const string RequiredPassword = "Informe o campo senha.";

        /*CUSTOM MESSAGES*/
        public const string ProfileBlocked = "Usuário bloqueado. entre em contato com suporte";
        public const string UserAdministratorBlocked = "Acesso bloqueado. entre em contato com suporte";
        public const string ProfileUnRegistred = "Informe um e-mail registrado.";
        public const string AwaitApproval = "Seu perfil está sendo analizado, logo entraremos em contato";
        public const string PasswordNoMatch = "Senha atual não confere com a informada";
        public const string ConfirmPasswordNoMatch = "Confirmação de senha não confere com a nova senha informada";
        public const string InvalidRegisterAddress = "Não é possível registrar esse tipo de endereço.";
        public const string AmountPhoto = "A foto deve ter no mínimo 1 e no máximo 15 imagens";
        public const string AmountPhotoInclude = "A inclusão das fotos está acima do máximo de 15 imagens";
        public const string ChangeSubject = "A mudança de assunto só pode ser feita se a situação estiver finalizada.";

        /*IN USE*/
        public const string CpfInUse = "Cpf em uso.";
        public const string CarPlateInUse = "Placa em uso.";
        public const string CnpjInUse = "Cnpj em uso.";
        public const string PhoneInUse = "Telefone em uso.";
        public const string LoginInUse = "Login em uso.";
        public const string EmailInUse = "Email em uso.";
        public const string FacebookInUse = "Ja existe um usuário com essa conta do facebook.";
        public const string GoogleIdInUse = "Ja existe um usuário com essa conta do google plus.";
        public const string MemberInUse = "Membro da família já cadastrado";
        public const string FamilyInUse = "Família já registrou interesse nesse imóvel";

        /*INVALID*/
        public const string InvalidCredencials = "Credênciais inválidas.";
        public const string EmailInvalid = "Informe um email válido.";
        public const string CpfInvalid = "Informe um cpf válido.";
        public const string EnumInvalid = "Enumerado inválido.";

        public const string CnpjInvalid = "Informe um cnpj válido.";
        public const string PhoneInvalid = "Informe um telefone válido.";
        public const string InvalidIdentifier = "Formato de id inválido.";
        public const string InvalidEntityMap = "Mapeamento inválido.";
        public const string InvalidLogin = "Login e/ou senha inválidos.";
        public const string InvalidCredentials = "Credênciais inválidas.";
        public const string DateInvalidToSchedule = "A data deve ser posterior a atual.";
        public const string DateInvalid = "Data Inválida.";
        public const string ScheduleNotInChooseProperty = "Família não pode escolher o imóvel, pois não está na fase correspondente.";
        public const string ResidencialSaled = "O morador não pode ter dois ou mais imóveis comprados.";

        /*NOT FOUND*/
        public const string UserAdministratorNotFound = "Usuário de acessso não encontrado";
        public const string CpfNotFound = "Cpf não encontrado.";
        public const string ProfileNotFound = "Usuário não encontrado";
        public const string CourseNotFound = "Curso não encontrado";
        public const string CourseFamilyNotFound = "Família não encontrada para este curso";
        public const string ResidencialPropertyNotFound = "Propriedade não encontrado";
        public const string AnyResidencialProperty = "Nenhuma propriedade encontrada.";
        public const string QuizNotFound = "Quiz não encontrado";
        public const string AnswerNotFound = "Resposta(s) não encontrada(s)";
        public const string QuizFamilyNotFound = "Não foi encontrado essa disponibilização de questionário para a família";
        public const string DescriptionNotFound = "Descrição não encontrado";
        public const string QuestionNotFound = "Quiz não encontrado";
        public const string QuestionIdNotFound = "Necessário passar o Id da Questão";
        public const string FamilyNotFound = "Família não encontrado";
        public const string ScheduleNotFound = "Agendamento não encontrado";
        public const string AnyFamilyNotFound = "Alguma família não encontrado";
        public const string MemberNotFound = "Membro da família não encontrado";
        public const string PhotoNotFound = "Membro da família não encontrado";
        public const string CreditCardNotFound = "Cartão não encontrado";
        public const string CreditCardNotFoundIugu = "Forma de pagamento não encontrada";
        public const string ZipCodeNotFound = "Cep não encontrado";
        public const string Updated = "Alterações salvas com sucesso";
        public const string Registred = "Cadastrado com sucesso";
        public const string Deleted = "Removido com sucesso";
        public const string EmptyProviderId = "Informe o providerId";
        public const string AppleIdInUse = "AppleId em uso, tente fazer login.";
        public const string AccessBlocked = "Acesso bloqueado. entre em contato com suporte";
        public const string AccessBlockedWithReason = "Acesso bloqueado. entre em contato com suporte, {0}";
        public const string OnlyAdministrator = "Você precisa ser administrador para realizar essa ação";
        public const string MessageException = "Ocorreu um erro, verifique e tente novamente";
        public const string LogActionNotFound = "Log de ação não encontrado.";
        public const string VideoNotFound = "Vídeo não encontrado.";
        public const string InformativeNotFound = "Informativo não encontrado.";
        public const string NotificationNotFound = "Notificação não encontrada.";
        public const string LocationNotFound = "Localização não encontrada.";
        public const string PropertySaledNotFound = "Propriedade vendida não encontrada.";
        public const string ChoiceLimitExceeded = "Limite de escolha excedido.";
        public const string FieldSearchTerm = "Termo de busca";
        public const string DeviceInUse = "Já existe um login em outro dispositivo";
        public const string InvalidDeviceID = "Você iniciou uma nova sessão em outro device";
        public const string OldVersion = "App versão antiga";
        public const string Success = "Sucesso";
        public const string FileNotFound = "Nenhum arquivo encontrado";
        public const string FileNotAllowed = "Arquivo não permitido";
        public const string AnyQuiz = "Nenhum questionário encontrado";
        public const string ZeroItems = "Nenhum item importado";
        public const string PropertyNotChosen = "Imóvel de interesse não escolhido";
        public const string StageInvalidToSchedule = "São necessárias 03 reuniões PGM antes de prosseguir";
        public const string StageInvalidToPGM = "Todas as reuniões PGM foram agendadas";
        public const string StageInOpen = "Você precisa finalizar todas reuniões PGM em aberto";
        public const string AlreadySelectedForFamily = "Esse imóvel já foi contemplado para outra família.";
        public const string NoPropertiesInterest = "Nenhum interesse registrado para esse imóvel.";
        public const string SelectPropertyAvailable = "Escolha de imóvel liberada";
        public const string EmailNotFound = "Familia sem e-mail cadastrado";
        public const string FamilyHasPropertySelected = "Essa familia já foi contemplada com outro imóvel";
       

    }
}