export interface IEntity {
  id: string;
}

export default class Entity implements IEntity {
  constructor(id: string) {
    this.id = id;
  }

  public id: string;
}
