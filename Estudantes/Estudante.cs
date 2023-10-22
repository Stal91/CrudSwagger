namespace ApiCrud.Estudantes
{
    public class Estudante
    {
        public Guid Id { get; set; }
        public string Name { get; private set; }
        public bool Ativo { get; private set; }

        public Estudante(string name) 
        {
            Name = name;
            Id = Guid.NewGuid();
            Ativo = true;
        }

        public void RefreshName(string name)
        {
            Name = name;
        }

        public void Disable()
        {
            Ativo = false;
        }
    }
}
