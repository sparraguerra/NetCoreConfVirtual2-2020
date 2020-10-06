export interface GadaDocument {
	id: string;
	uri: string;
	documentType: string;
	fileName: string;
	size: number;
	contentType: string;
	created: Date;
	createdUser: string;
	properties: string[];
}
