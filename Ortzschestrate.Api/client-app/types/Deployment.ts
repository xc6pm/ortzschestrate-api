import type { Address } from "viem"

export interface Deployment {
  address: Address
  abi?: any
  transactionHash: string
  receipt: Receipt
  args?: null[] | null
  numDeployments: number
  solcInputHash: string
  metadata: string
  bytecode: string
  deployedBytecode: string
  devdoc: Devdoc
  userdoc: Userdoc
  storageLayout: StorageLayout
}
export interface AbiEntity {
  inputs?: (InputsEntity | null)[] | null
  stateMutability?: string | null
  type: string
  name?: string | null
  anonymous?: boolean | null
  outputs?: (OutputsEntity | null)[] | null
}
export interface InputsEntity {
  internalType: string
  name: string
  type: string
  indexed?: boolean | null
}
export interface OutputsEntity {
  components?: InputsEntityOrOutputsEntityOrComponentsEntity[] | null
  internalType: string
  name: string
  type: string
}
export interface InputsEntityOrOutputsEntityOrComponentsEntity {
  internalType: string
  name: string
  type: string
}
export interface Receipt {
  to?: null
  from: string
  contractAddress: string
  transactionIndex: number
  gasUsed: string
  logsBloom: string
  blockHash: string
  transactionHash: string
  logs?: LogsEntity[] | null
  blockNumber: number
  cumulativeGasUsed: string
  status: number
  byzantium: boolean
}
export interface LogsEntity {
  transactionIndex: number
  blockNumber: number
  transactionHash: string
  address: string
  topics?: string[] | null
  data: string
  logIndex: number
  blockHash: string
}
export interface Devdoc {
  kind: string
  version: number
}
export interface Userdoc {
  kind: string
  methods: Methods1
  version: number
}
export interface Methods1 {}
export interface StorageLayout {
  storage?: StorageEntityOrMembersEntity[] | null
  types: Types
}
export interface StorageEntityOrMembersEntity {
  astId: number
  contract: string
  label: string
  offset: number
  slot: string
  type: string
}
export interface Types {
  t_address: TAddressOrTBoolOrTBytes32OrTUint256
  t_bool: TAddressOrTBoolOrTBytes32OrTUint256
  t_bytes32: TAddressOrTBoolOrTBytes32OrTUint256
}
export interface TAddressOrTBoolOrTBytes32OrTUint256 {
  encoding: string
  label: string
  numberOfBytes: string
}
