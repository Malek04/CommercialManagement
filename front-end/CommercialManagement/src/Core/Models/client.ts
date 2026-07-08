export class Client {
  id!: string;
  lastName?: string;
  firstName?: string;
  email?: string;
  phone?: string;
  adresse: Adresse = new Adresse();
  created!: Date;
}
export class Adresse {
  rue?: string;
  ville?: string;
  codePostal?: string;
  pays?: string;
}