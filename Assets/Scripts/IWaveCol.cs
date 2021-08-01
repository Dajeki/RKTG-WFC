public interface IWaveCol {
	abstract void propogateGrid();
	abstract Tile[] getLeastEntropic();
	abstract void initSuperposition();
}