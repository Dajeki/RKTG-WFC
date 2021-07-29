public interface IWaveCol {
	abstract void propogateGrid( int sideNumber );
	abstract Tile[] getLeastEntropic();
	abstract void initSuperposition();
}