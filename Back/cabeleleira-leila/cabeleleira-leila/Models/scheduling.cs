using cabeleleira_leila.Enums;

namespace cabeleleira_leila.Models;

public class Scheduling : BaseModel
{
    public long ClienteId { get; set; }
    public User User { get; set; } = null!;
    public DateTime DataHora { get; set; }
    public StatusAgendamento Status { get; set; }
    public ICollection<Servico> Servicos { get; set; } = [];

    protected Scheduling()
    {
    }

    public Scheduling(long clienteId, DateTime dataHora, IEnumerable<Servico> servicos)
    {
        ClienteId = clienteId;
        DataHora = dataHora;
        Status = StatusAgendamento.Agendado;
        Servicos = servicos.ToList();
    }

    public void Update(DateTime dataHora, StatusAgendamento status, IEnumerable<Servico> servicos)
    {
        DataHora = dataHora;
        Status = status;
        Servicos.Clear();

        foreach (var servico in servicos)
        {
            Servicos.Add(servico);
        }
    }

    public void UpdateStatus(StatusAgendamento status)
    {
        Status = status;
    }
}
