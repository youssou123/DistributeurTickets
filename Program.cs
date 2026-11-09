using System;
using System.Collections.Generic;
using System.IO;

namespace DistributeurTickets
{
    class Client
    {
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string NumeroCompte { get; set; } = string.Empty;
        public string TypeOperation { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
    }

    internal class Program
    {
        private static readonly string CheminFichierNumero = Path.Combine(Path.GetTempPath(), "fnuméro.txt");

        private static int compteurVersement = 0;
        private static int compteurRetrait = 0;
        private static int compteurInfo = 0;

        private static readonly List<Client> listeClients = new List<Client>();

        static void Main(string[] args)
        {
            InitialiserFichierNumero();

            bool continuer = true;

            Console.WriteLine("==================================================");
            Console.WriteLine("    DISTRIBUTEUR AUTOMATIQUE DE TICKETS BANQUE    ");
            Console.WriteLine("==================================================\n");

            while (continuer)
            {
                Console.WriteLine("Choisissez le type d'opération :");
                Console.WriteLine("1. Versement (V)");
                Console.WriteLine("2. Retrait (R)");
                Console.WriteLine("3. Informations (I)");
                Console.Write("Votre choix (1, 2 ou 3) : ");
                string choix = Console.ReadLine()?.Trim() ?? "";

                string prefixe = "";
                string typeOperation = "";
                int rangClient = 0;

                switch (choix)
                {
                    case "1":
                        prefixe = "V";
                        typeOperation = "Versement";
                        compteurVersement++;
                        rangClient = compteurVersement;
                        break;
                    case "2":
                        prefixe = "R";
                        typeOperation = "Retrait";
                        compteurRetrait++;
                        rangClient = compteurRetrait;
                        break;
                    case "3":
                        prefixe = "I";
                        typeOperation = "Informations";
                        compteurInfo++;
                        rangClient = compteurInfo;
                        break;
                    default:
                        Console.WriteLine("\n[X] Choix invalide ! Veuillez recommencer.\n");
                        continue;
                }

                Console.Write("Numéro de compte : ");
                string numCompte = Console.ReadLine()?.Trim() ?? "";

                Console.Write("Nom : ");
                string nom = Console.ReadLine()?.Trim() ?? "";

                Console.Write("Prénom : ");
                string prenom = Console.ReadLine()?.Trim() ?? "";

                string numeroTicket = $"{prefixe}-{rangClient}";

                SauvegarderCompteursDansFichier();

                Client clientCourant = new Client
                {
                    Nom = nom,
                    Prenom = prenom,
                    NumeroCompte = numCompte,
                    TypeOperation = typeOperation,
                    Ticket = numeroTicket
                };
                listeClients.Add(clientCourant);

                int personnesAvant = rangClient - 1;
                string messageAttente = FormaterMessageAttente(numeroTicket, personnesAvant);
                Console.WriteLine($"\n--> {messageAttente}\n");

                Console.Write("Voulez-vous prendre un autre numéro pour un nouveau client ? (o/n) : ");
                string reponse = Console.ReadLine()?.Trim().ToLower() ?? "";
                Console.WriteLine();

                if (reponse != "o" && reponse != "oui")
                {
                    continuer = false;
                }
            }

            AfficherRecapitulatifClients();
        }

        private static void InitialiserFichierNumero()
        {
            try
            {
                if (File.Exists(CheminFichierNumero))
                {
                    string[] lignes = File.ReadAllLines(CheminFichierNumero);
                    if (lignes.Length >= 3)
                    {
                        int.TryParse(lignes[0], out compteurVersement);
                        int.TryParse(lignes[1], out compteurRetrait);
                        int.TryParse(lignes[2], out compteurInfo);
                    }
                }
                else
                {
                    SauvegarderCompteursDansFichier();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Erreur Fichier] {ex.Message}");
            }
        }

        private static void SauvegarderCompteursDansFichier()
        {
            string contenu = $"{compteurVersement}\n{compteurRetrait}\n{compteurInfo}";
            File.WriteAllText(CheminFichierNumero, contenu);
        }

        private static string FormaterMessageAttente(string ticket, int personnesAvant)
        {
            string patronMessage = "Votre numéro est {0}, et il y'a {1} personnes qui attendent avant vous.";
            return string.Format(patronMessage, ticket, personnesAvant);
        }

        private static void AfficherRecapitulatifClients()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("       RÉCAPITULATIF DE LA SESSION CLIENTS        ");
            Console.WriteLine("==================================================");

            if (listeClients.Count == 0)
            {
                Console.WriteLine("Aucun client enregistré.");
                return;
            }

            foreach (var client in listeClients)
            {
                Console.WriteLine($"Ticket: {client.Ticket,-6} | Nom: {client.Prenom} {client.Nom,-15} | Compte: {client.NumeroCompte,-10} | Type: {client.TypeOperation}");
            }
            Console.WriteLine("==================================================\n");
            Console.WriteLine($"Fichier des derniers numéros stocké ici : {CheminFichierNumero}");
        }
    }
}