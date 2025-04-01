using System.ComponentModel;
using System.Runtime.Serialization;

namespace Moralar.Data.Enum
{
    public enum RouteNotification
    {
        [EnumMember(Value = "Notificação de sistema")]
        [Description("Notificação de sistema")]
        System,
        [EnumMember(Value = "Agendamento")]
        [Description("Agendamento")]
        Schedule

    }

    public enum ForType
    {
        [EnumMember(Value = "Família")]
        [Description("Família")]
        Family = 0,
        [EnumMember(Value = "TTS")]
        [Description("TTS")]
        TTS = 1,
        [EnumMember(Value = "Gestor")]
        [Description("Gestor")]
        Gestor = 2,
        [EnumMember(Value = "Administrador")]
        [Description("Administrador")]
        Administrador = 3
    }

    public enum TypeProvider
    {
        [EnumMember(Value = "Senha")]
        [Description("Senha")]
        Password = 0,
        [EnumMember(Value = "Facebook")]
        [Description("Facebook")]
        Facebook = 1,
        [EnumMember(Value = "AppleId")]
        [Description("AppleId")]
        Apple = 2,
        [EnumMember(Value = "GoogleId")]
        [Description("GoogleId")]
        Google = 3
    }

    public enum TypeAccount
    {
        [EnumMember(Value = "Corrente")]
        [Description("Corrente")]
        CC = 0,
        [EnumMember(Value = "Poupança")]
        [Description("Poupança")]
        CP = 1

    }

    public enum TypePersonBank
    {
        [EnumMember(Value = "Pessoa Jurídica")]
        [Description("Pessoa Jurídica")]
        LegalPerson = 0,
        [EnumMember(Value = "Pessoa Física")]
        [Description("Pessoa Física")]
        PhysicalPerson = 1
    }
    public enum TypeUserProfile
    {
        [EnumMember(Value = "Gestor")]
        [Description("Gestor")]
        Gestor = 0,
        [EnumMember(Value = "TTS")]
        [Description("TTS")]
        TTS = 1,
        [EnumMember(Value = "Admin")]
        [Description("Admin")]
        Admin = 2
    }
    public enum Language
    {
        [EnumMember(Value = "Ingles")]
        [Description("Ingles")]
        En = 0,
        [EnumMember(Value = "Espanhol")]
        [Description("Espanhol")]
        Es = 1,
        [EnumMember(Value = "Português")]
        [Description("Português")]
        Pt = 2

    }

    public enum TypeProperty
    {
        [EnumMember(Value = "Casa")]
        [Description("Casa")]
        Casa = 0,
        [EnumMember(Value = "Apartamento")]
        [Description("Apartamento")]
        Apartamento = 1
    }
    public enum TypeAffirmation
    {
        [EnumMember(Value = "Sim")]
        [Description("Sim")]
        Sim = 0,
        [EnumMember(Value = "Não")]
        [Description("Não")]
        Nao = 1
    }
    public enum TypePropertyRegularization
    {
        [EnumMember(Value = "Regular")]
        [Description("Regular")]
        Regular = 0,
        [EnumMember(Value = "Regularizável")]
        [Description("Regularizável")]
        Regularizável = 1,
        [Description("Irregular")]
        Irregular = 2
    }

    public enum TypePropertyGasInstallation
    {
        [EnumMember(Value = "Gás encanado")]
        [Description("Gás encanado")]
        GasEncanado = 0,
        [EnumMember(Value = "Botijão")]
        [Description("Botijão")]
        Botijao = 1
    }
    public enum TypeStatusResidencial
    {
        [EnumMember(Value = "A Escolher")]
        [Description("A Escolher")]
        AEscolher = 0,
        [EnumMember(Value = "Vendido")]
        [Description("Vendido")]
        Vendido = 1,
        [EnumMember(Value = "Bloqueado")]
        [Description("Bloqueado")]
        Bloqueado = 2
    }
    public enum TypeProfile
    {
        [EnumMember(Value = "Usuário TTS")]
        [Description("Usuário TTS")]
        TTS = 0,
        [EnumMember(Value = "Gestor")]
        [Description("Gestor")]
        Gestor = 1,
        [EnumMember(Value = "Família")]
        [Description("Família")]
        Familia = 2,
        [EnumMember(Value = "Admin")]
        [Description("Admin")]
        Admin = 3

    }
    public enum TypeScholarity
    {
        [EnumMember(Value = "Não possui")]
        [Description("Não possui")]
        NaoPossui = 0,
        [EnumMember(Value = "Fundamental (incompleto)")]
        [Description("Fundamental (incompleto)")]
        FundamentalIncompleto = 1,
        [EnumMember(Value = "Fundamental (completo)")]
        [Description("Fundamental (completo)")]
        FundamentalCompleto = 2,
        [EnumMember(Value = "Médio (incompleto)")]
        [Description("Médio (incompleto)")]
        MedioIncompleto = 3,
        [EnumMember(Value = "Médio (completo)")]
        [Description("Médio (completo)")]
        MedioCompleto = 4,
        [EnumMember(Value = "Superior (incompleto)")]
        [Description("Superior (incompleto)")]
        SuperiorIncompleto = 5,
        [EnumMember(Value = "Superior (completo)")]
        [Description("Superior (completo)")]
        SuperiorCompleto = 6,
        [EnumMember(Value = " Pós-graduação (incompleto)")]
        [Description(" Pós-graduação (incompleto)")]
        PosGraduaçãoIncompleto = 7,
        [EnumMember(Value = "Pós-graduação (completo")]
        [Description("Pós-graduação (completo")]
        PosGraduaçãoCompleto = 8
    }
    public enum TypeKingShip
    {
        [EnumMember(Value = "Filha")]
        [Description("Filha")]
        Filha = 0,
        [EnumMember(Value = "Filho")]
        [Description("Filho")]
        Filho = 1,
        [EnumMember(Value = "Mãe")]
        [Description("Mãe")]
        Mae = 2,
        [EnumMember(Value = "Pai")]
        [Description("Pai")]
        Pai = 3,
        [EnumMember(Value = "Avó")]
        [Description("Avó")]
        AvoMulher = 4,
        [EnumMember(Value = "Avô")]
        [Description("Avô")]
        AvoHomem = 5,
        [EnumMember(Value = "Enteada")]
        [Description("Enteada")]
        Enteada = 6,
        [EnumMember(Value = " Enteado")]
        [Description("Enteado")]
        Enteado = 7,
        [EnumMember(Value = "Tia")]
        [Description("Tia")]
        Tia = 8,
        [EnumMember(Value = "Tio")]
        [Description("Tio")]
        Tio = 9,
        [EnumMember(Value = "Outro")]
        [Description("Outro")]
        Outro = 10
    }
    public enum TypeAction
    {
        [EnumMember(Value = "Cadastro")]
        [Description("Cadastro")]
        Register,
        [EnumMember(Value = "Alteração")]
        [Description("Alteração")]
        Change,
        [EnumMember(Value = "Remoção")]
        [Description("Remoção")]
        Delete,
        [EnumMember(Value = "Upload")]
        [Description("Upload")]
        Upload,
        [EnumMember(Value = "Download")]
        [Description("Download")]
        Download,
        [EnumMember(Value = "Block")]
        [Description("Block")]
        Block,
        [EnumMember(Value = "UnBlock")]
        [Description("UnBlock")]
        UnBlock,
        [EnumMember(Value = "Importação de Família")]
        [Description("Importação de Família")]
        ImportFamily,
    }
    public enum TypeResposible
    {
        [EnumMember(Value = "Gestor")]
        [Description("Gestor")]
        Gestor = 0,
        [EnumMember(Value = "Usuário administrador")]
        [Description("Usuário administrador")]
        UserAdminstrator = 1,
        [EnumMember(Value = "Usuário administrador/gestor")]
        [Description("Usuário administrador/gestor")]
        UserAdminstratorGestor = 2
    }
    public enum TypeGenre
    {
        [EnumMember(Value = "Feminino")]
        [Description("Feminino")]
        Feminino = 0,
        [EnumMember(Value = "Masculino")]
        [Description("Masculino")]
        Masculino = 1,
        [EnumMember(Value = "Outro")]
        [Description("Outro")]
        Outro = 2,
        [EnumMember(Value = "Todos")]
        [Description("Todos")]
        Todos = 3
    }
    public enum LocalAction
    {
        [EnumMember(Value = "Família")]
        [Description("Família")]
        Familia = 0,
        [EnumMember(Value = "Propriedade Residencial")]
        [Description("Propriedade Residencial")]
        ResidencialProperty = 1,
        [EnumMember(Value = "Questionário")]
        [Description("Questionário")]
        Question = 2,
        [EnumMember(Value = "Perfil")]
        [Description("Perfil")]
        Perfil = 3,
        [EnumMember(Value = "Curso")]
        [Description("Curso")]
        Curso = 4,
        [EnumMember(Value = "Notificação")]
        [Description("Notificação")]
        Notificacao = 4,
        [EnumMember(Value = "Informativo")]
        [Description("Informativo")]
        Informativo = 4

    }
    public enum TypeQuiz
    {
        [EnumMember(Value = "Quiz")]
        [Description("Quiz")]
        Quiz = 0,
        [EnumMember(Value = "Enquete")]
        [Description("Enquete")]
        Enquete = 1
    }
    public enum TypeResponse
    {
        [EnumMember(Value = "Texto")]
        [Description("Texto")]
        Texto = 0,
        [EnumMember(Value = "Múltipla escolha")]
        [Description("Múltipla escolha")]
        MultiplaEscolha = 1,
        [EnumMember(Value = "Escolha única")]
        [Description("Escolha única")]
        EscolhaUnica = 2,
        [EnumMember(Value = "Lista suspensa")]
        [Description("Lista suspensa")]
        ListaSuspensa = 3
    }
    public enum TypeStatusCourse
    {
        [EnumMember(Value = "Inscrito")]
        [Description("Inscrito")]
        Inscrito = 0,
        [EnumMember(Value = "Aguardando na lista de espera")]
        [Description("Aguardando na lista de espera")]
        ListaEspera = 1
    }
    public enum TypeStatusActiveInactive
    {
        [EnumMember(Value = "Ativo")]
        [Description("Ativo")]
        Ativo = 0,
        [EnumMember(Value = "Inativo")]
        [Description("Inativo")]
        Inativo = 1
    }
    public enum TypeStatus
    {

        [EnumMember(Value = "Não Respondido")]
        [Description("Não Respondido")]
        NaoRespondido = 0,
        [EnumMember(Value = "Respondido")]
        [Description("Respondido")]
        Respondido = 1

    }
    public enum TypeSubject
    {
        [EnumMember(Value = "Visita do TTS")]
        [Description("Visita do TTS")]
        VisitaTTS = 0,
        [EnumMember(Value = "Reunião com TTS")]
        [Description("Reunião com TTS")]
        ReuniaoTTS = 1,
        [EnumMember(Value = "Reunião PGM")]
        [Description("Reunião PGM")]
        ReuniaoPGM = 2,
        [EnumMember(Value = "Visita ao imóvel")]
        [Description("Visita ao imóvel")]
        VisitaImovel = 3,
        [EnumMember(Value = "Escolha do imóvel")]
        [Description("Escolha do imóvel")]
        EscolhaDoImovel = 4,
        [EnumMember(Value = "Demolição")]
        [Description("Demolição")]
        Demolicão = 5,
        [EnumMember(Value = "Outros")]
        [Description("Outros")]
        Outros = 6,
        [EnumMember(Value = "Mudança")]
        [Description("Mudança")]
        Mudanca = 7,
        [EnumMember(Value = "Acompanhamento pós-mudança")]
        [Description("Acompanhamento pós-mudança")]
        AcompanhamentoPosMudança = 8
    }
    public enum TypeScheduleStatus
    {
        [EnumMember(Value = "Aguardando confirmação")]
        [Description("Aguardando confirmação")]
        AguardandoConfirmacao = 0,  
        [EnumMember(Value = "Confirmado")]
        [Description("Confirmado")]
        Confirmado = 1,
        [EnumMember(Value = "Aguardando reagendamento")]
        [Description("Aguardando reagendamento")]
        AguardandoReagendamento = 2,
        [EnumMember(Value = "Reagendado")]
        [Description("Reagendado")]
        Reagendado = 3,
        [EnumMember(Value = "Finalizado")]
        [Description("Finalizado")]
        Finalizado = 4,
        [Description("Cancelado")]
        Cancelado = 5
    }
    public enum TypeReadUnread
    {
        [EnumMember(Value = "Não Lido")]
        [Description("Não Lido")]
        NaoLido = 0,
        [EnumMember(Value = "Lido")]
        [Description("Lido")]
        Lido = 1,
    }
    public enum TypeNotification
    {
        [EnumMember(Value = "Manual")]
        [Description("Manual")]
        Manual = 0,
        [EnumMember(Value = "Automática")]
        [Description("Automática")]
        Automatica = 1,
    }

    public enum TypeErrorExcel
    {
        [EnumMember(Value = "Null")]
        [Description("Null")]
        Null = 0,
        [EnumMember(Value = "Numeric")]
        [Description("Numeric")]
        Numeric = 1,
        [EnumMember(Value = "InvalidFormat")]
        [Description("InvalidFormat")]
        InvalidFormat = 2,
        [EnumMember(Value = "WrongDate")]
        [Description("WrongDate")]
        WrongDate = 3,
    }


}